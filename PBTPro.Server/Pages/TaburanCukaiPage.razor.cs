using DevExpress.XtraPrinting.Native;
using GoogleMapsComponents;
using GoogleMapsComponents.Maps;
using Microsoft.AspNetCore.Components;
using GoogleMapsComponents.Maps.Coordinates;
using GoogleMapsComponents.Maps.Extension;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleMapsComponents.Maps.Places;
using DevExpress.ClipboardSource.SpreadsheetML;
using PBTPro.DAL.Models;
using DevExpress.DashboardCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBTPro.Data;
using System.Collections.Concurrent;


namespace PBTPro.Pages
{
    public partial class TaburanCukaiPage
    {

        //For check box ===========
        public List<FilterData> FilterList { get; set; }
        protected List<string> SelectedIds = new List<string>();
        public List<FilterData> ObjectList { get; set; }
        public string OutPutValue { get; set; }

        public List<FilterData> NotaList { get; set; }
        //=======================

        private string LesenID { get; set; } = string.Empty;
        private string _labelText = "";

        //Lesen Information
        IEnumerable<NoticeProp> NoticeData;
        IEnumerable<(NoticeProp, int)> Items;
        TaskCompletionSource<bool> DataLoadedTcs { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);


        int intLegend;
        private Autocomplete autocomplete;
        private string message;
        private ElementReference searchBox;

        //Map
        private GoogleMap map1;
        private MapOptions _mapOptions;

        private ControlPosition _controlPosition = ControlPosition.TopLeft;
        [Inject] private IJSRuntime JsRuntime { get; set; }
        public int ZIndex { get; set; } = 0;
        private readonly Stack<Marker> markersPin = new Stack<Marker>();
        private readonly Stack<AdvancedMarkerElement> markers = new Stack<AdvancedMarkerElement>();

        private LatLngBounds _bounds = null!;
        private MarkerClustering? _markerClustering;
        private List<AdvancedMarkerElement>? _clusteringMarkers; // ismail changing to list - this list can be append when have new point from api

        // 07/11/2024 - to reduce API request frequency - ismail
        private DateTime _lastMoveTime;
        private readonly TimeSpan _throttleTime = TimeSpan.FromMilliseconds(300);
        private ConcurrentDictionary<int, bool> _processedLotGids = new ConcurrentDictionary<int, bool>(); // Thread-safe GID tracking
        private ConcurrentDictionary<int, bool> _processedPremisGids = new ConcurrentDictionary<int, bool>(); // Thread-safe GID tracking
        private readonly object _lockLot = new object();
        private bool _isProcessing = false;  // Flag to track if tasks are processing
        private Queue<Task> _pendingTasks = new Queue<Task>();

        //Legend
        protected ElementReference LegendReference { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await DataLoadedTcs.Task;
            }
        }

        string GetColorLot(int typeLot)
        {
            switch (typeLot)
            {
                case 1:
                    return "Green";
                case 2:
                    return "Red";
                case 3:
                    return "Blue";
                case 4:
                    return "Orange";
                case 5:
                    return "Yellow";
                case 6:
                    return "#ccc";
                case 7:
                    return "Purple";
                default:
                    return "#ccc";
            }
        }

        string GetIcon(string color)
        {
            string svgTemplate =
                        @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""26"" height=""26"" viewBox=""0 0 30 30"">
                <circle cx=""15"" cy=""15"" r=""5"" fill=""{0}""/></svg>";
            string svgContent = string.Format(svgTemplate, color);
            string urlEncodedSvg = Uri.EscapeDataString(svgContent);
            string dataUrl = $"data:image/svg+xml,{urlEncodedSvg}";
            return dataUrl;
        }

        /*
        Description: Set the map properties
        Author: Azmee
        Date: November 2024
        Version: 1.0
        */
        protected override async Task OnInitializedAsync()
        {
            //Show legend
            intLegend = 0;
            //Map
            _mapOptions = new MapOptions
            {
                Zoom = 13,
                ZoomControlOptions = new ZoomControlOptions
                {
                    Position = ControlPosition.RightBottom
                },
                Center = new LatLngLiteral
                {
                    Lat = 3.0501028427254098,
                    Lng = 101.62482171721311
                },
                IsFractionalZoomEnabled = false,
                HeadingInteractionEnabled = false,
                CameraControl = true,
                MapTypeId = MapTypeId.Roadmap,
                MapId = "a68e487c98db929b", //"e5asd595q2121",  //"67679fd72b783792" <-- vector :: caused error,
                //RenderingType = RenderingType.Vector,
                MapTypeControlOptions = new MapTypeControlOptions
                {
                    Position = ControlPosition.RightTop,
                    Style = MapTypeControlStyle.HorizontalBar,
                    MapTypeIds = new[] { MapTypeId.Roadmap, MapTypeId.Terrain, MapTypeId.Satellite, MapTypeId.Hybrid }
                },
                //StreetViewControl = true,
                FullscreenControlOptions = new FullscreenControlOptions
                {
                    Position = ControlPosition.RightTop
                }
            };


            //For check box
            FilterList = GetMockFilter();
            NotaList = GetNotaFilter();
        }

        private async Task OnAfterMapInit()
        {
            try
            {
                IJSObjectReference serverSideScripts3 = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/firepanel/main.js");

                this.autocomplete = await Autocomplete.CreateAsync(this.map1.JsRuntime, this.searchBox, new AutocompleteOptions
                {
                    StrictBounds = false,
                });

                await this.autocomplete.SetFields(new[] { "address_components", "geometry", "name" });
                await this.autocomplete.AddListener("place_changed", async () =>
                {
                    var place = await this.autocomplete.GetPlace();

                    if (place?.Geometry == null)
                    {
                        this.message = "No results available for " + place?.Name;
                    }
                    else if (place.Geometry.Location != null)
                    {
                        await this.map1.InteropObject.SetCenter(place.Geometry.Location);
                        await this.map1.InteropObject.SetZoom(18);

                        var marker = await Marker.CreateAsync(this.map1.JsRuntime, new MarkerOptions
                        {
                            Position = place.Geometry.Location,
                            Map = this.map1.InteropObject,
                            Title = place.Name
                        });

                        this.markersPin.Push(marker);

                        this.message = "Displaying result for " + place.Name;
                    }
                    else if (place.Geometry.Viewport != null)
                    {
                        await this.map1.InteropObject.FitBounds(place.Geometry.Viewport, 5);
                        this.message = "Displaying result for " + place.Name;
                    }

                    this.StateHasChanged();
                });

                _bounds = await LatLngBounds.CreateAsync(map1.JsRuntime);

                //Get record
                NoticeData = await _NoticeService.GetNoticeAsync();
                //Items = NoticeData.Select((item, index) => (item, index));
                DataLoadedTcs.TrySetResult(true);
                //Start populate the map
                await InvokeClustering(1);

                // Add listener for map api polygon data -- ismail
                await ProcessMapAPIData();
                await this.map1.InteropObject.AddListener("dragend", async () => await ProcessMapAPIData());
                await this.map1.InteropObject.AddListener("zoom_changed", async () => await ProcessMapAPIData());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnAfterMapInit error : {ex.Message}");
            }

        }

        private async Task InvokeClustering(int initStart)
        {
            _clusteringMarkers = await populateMarker(initStart);

            _markerClustering = await MarkerClustering.CreateAsync(map1.JsRuntime, map1.InteropObject, _clusteringMarkers, new()
            {
                //RendererObjectName = "customRendererLib.interpolatedRenderer",
                ZoomOnClick = true,
            });


            foreach (var _notice in NoticeData)
            {
                await _bounds.Extend(_notice.Position);
            }

            var boundsLiteral = await _bounds.ToJson();
            await map1.InteropObject.FitBounds(boundsLiteral, OneOf.OneOf<int, Padding>.FromT0(5));
        }

        private async Task<List<AdvancedMarkerElement>> populateMarker(int initStart)
        {
            var result = new List<AdvancedMarkerElement>(NoticeData.Count());
            bool blnValidFilter = true;
            foreach (var _notice in NoticeData)
            {
                blnValidFilter = true;
                if (initStart != 1)
                {
                    blnValidFilter = false;
                    if (SelectedIds.Contains(_notice.Type.ToString()))
                    {
                        blnValidFilter = true;
                    }
                }

                //Start filtering based on selected tapisan
                if (blnValidFilter)
                {
                    var _marker = await AdvancedMarkerElement.CreateAsync(map1.JsRuntime, new AdvancedMarkerElementOptions()
                    {
                        Position = _notice.Position,
                        Map = map1.InteropObject,
                        Title = _notice.NoLesen,
                        // Content = index.ToString()
                        Content = @"<div><svg xmlns=""http://www.w3.org/2000/svg"" width=""26"" height=""26"" viewBox=""0 0 30 30"">
                    <circle cx=""15"" cy=""15"" r=""5"" fill='" + GetColorLot(_notice.Type) + "'/></svg><lable class='map-marker-label'>" + $"{_notice.NoLot}" + "</lable></div>",
                    });

                    markers.Push(_marker);

                    await _marker.AddListener<MouseEvent>("click", async e =>
                    {
                        //string markerLabelText = await marker.GetLabelText();
                        //string _title = await _marker.GetTitle();
                        // _events.Add("click on " + _title);
                        await OpenSideBar(_notice.NoLesen);
                        StateHasChanged();
                        ///await e.Stop();
                    });

                    result.Add(_marker);
                }

                //Add all selected filter
                if (initStart == 1) // first init
                {
                    if (!SelectedIds.Contains(_notice.Type.ToString()))
                    {
                        SelectedIds.Add(_notice.Type.ToString());
                    }
                }

            }

            return result;
        }

        private async Task ClearClustering()
        {
            if (_markerClustering == null || _clusteringMarkers == null)
            {
                return;
            }

            await _markerClustering.RemoveMarkers(_clusteringMarkers);
        }

        private async Task AddLegend()
        {
            //////await map1.InteropObject.AddControl(_controlPosition, LegendReference);
            intLegend++;

            if (intLegend == 1)
                await map1.InteropObject.AddControl(_controlPosition, LegendReference);
            else
            {
                intLegend = 0;
                await map1.InteropObject.RemoveControl(_controlPosition, LegendReference);
            }

            //await OpenSideBar("Test");
        }

        private async Task OpenSideBar(string msg)
        {
            //Populate all the value from parameter. ex:LesenID
            // await JsRuntime.InvokeVoidAsync("alert", msg);
            _labelText = msg;
            // await JsRuntime.InvokeVoidAsync("openRightBar");
            IJSObjectReference serverSideScripts4 = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/main.js");
            await serverSideScripts4.InvokeVoidAsync("openRightBar");
        }

        private async Task OpenFilter()
        {
            IJSObjectReference serverSideScripts1 = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/main.js");
            await serverSideScripts1.InvokeVoidAsync("openNav");
        }

        private async Task CloseFilter()
        {
            IJSObjectReference serverSideScripts2 = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/main.js");
            await serverSideScripts2.InvokeVoidAsync("closeNav");
        }

        //Check Box
        protected async Task ShowSelectedValues()
        {
            await ClearClustering();
            //Populate back the filtering
            if (SelectedIds != null)
                await InvokeClustering(2);

            OutPutValue = string.Join(",", SelectedIds.ToArray());
            StateHasChanged();
        }
        private List<FilterData> GetMockFilter()
        {

            var vSubOne = new FilterData()
            {
                TypeId = 1,
                Description = "Lesen Aktif",
                Color = "Green"
            };
            var vSubTwo = new FilterData()
            {
                TypeId = 2,
                Description = "Lesen Tamat Tempoh",
                Color = "Red"
            };
            var vSubThree = new FilterData()
            {
                TypeId = 3,
                Description = "Tidak Berlesen",
                Color = "Blue"
            };
            var vSubFour = new FilterData()
            {
                TypeId = 4,
                Description = "Lesen Batal",
                Color = "Orange"
            };
            var vSubFive = new FilterData()
            {
                TypeId = 5,
                Description = "Lesen Gantung",
                Color = "Yellow"
            };
            var vSubSix = new FilterData()
            {
                TypeId = 6,
                Description = "Lot Kosong",
                Color = "#ccc"
            };

            var vSubList = new List<FilterData>
            {
                vSubOne, vSubTwo,
                vSubThree, vSubFour,
                vSubFive,vSubSix
            };

            return vSubList;
        }

        private List<FilterData> GetNotaFilter()
        {
            var vSubSeven = new FilterData()
            {
                TypeId = 7,
                Description = "Nota Pemeriksaan",
                Color = "Purple"
            };

            var vSubList = new List<FilterData>
            {
                vSubSeven
            };

            return vSubList;
        }

        public void Dispose()
        {
            DataLoadedTcs.TrySetCanceled();
        }

        #region Polygon section
        /*
        Description: This section for Polygon & Marker fetch from API
        Author: Ismail
        Date: November 2024
        Version: 1.0

        Additional Notes:
        - this will load data based on the visible map boundaries.
        - an initial draft might need to be changes after receive real data

        Changes Logs:
        07/11/2024 - initial create
        */
        private async Task DisableMapInteractivity()
        {
            try
            {
                var mapOptions = new MapOptions
                {
                    ZoomControl = false,
                    Draggable = false,
                    Scrollwheel = false
                };
                await this.map1.InteropObject.SetOptions(mapOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disabling interactivity: {ex.Message}");
            }
        }
        private async Task EnableMapInteractivity()
        {
            try
            {
                var mapOptions = new MapOptions
                {
                    ZoomControl = true,
                    PanControl = true,
                    Scrollwheel = true
                };

                await this.map1.InteropObject.SetOptions(mapOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enabling interactivity: {ex.Message}");
            }
        }

        private Task GenerateMapDataTask()
        {
            return Task.Run(() => ProcessMapAPIData());
        }

        private async Task ProcessMapAPIData()
        {
            try
            {
                if (DateTime.Now - _lastMoveTime < _throttleTime) return;
                _lastMoveTime = DateTime.Now;

                if (_isProcessing)  // If already processing, queue the new task
                {
                    _pendingTasks.Enqueue(GenerateMapDataTask());  // Enqueue the new task to process after current task
                    return;
                }

                var bounds = await this.map1.InteropObject.GetBounds();
                if (bounds != null)
                {
                    _isProcessing = true;
                    await GenerateLotData(bounds.South, bounds.West, bounds.North, bounds.East);
                    await GeneratePremisData(bounds.South, bounds.West, bounds.North, bounds.East);
                    _isProcessing = false;
                }

                if (_pendingTasks.Count > 0)
                {
                    var nextTask = _pendingTasks.Dequeue();
                    await nextTask;  // Process the next task in the queue
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ProcessMapAPIData error : {ex.Message}");
            }
            finally
            {
                //_isProcessing = false;  // Mark that processing is done
            }
        }

        private async Task GenerateLotData(double southLat, double westLng, double northLat, double eastLng)
        {
            try
            {
                string requestUrl = $"/api/Lot/GetListByBound?crs=4326&minLng={westLng}&minLat={southLat}&maxLng={eastLng}&maxLat={northLat}";
                var response = await _ApiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        var tasks = new List<Task>();
                        dynamic datas = JsonConvert.DeserializeObject(dataString);

                        List<dynamic> dataList = datas.ToObject<List<dynamic>>();
                        var filteredDatas = dataList.Where(d =>
                        {
                            int dataId = (int)d.gid;  // Convert gid to int (ensure it's valid)
                            return !_processedLotGids.ContainsKey(dataId);  // Only include items whose gid is not in _processedLotGids
                        }).ToList();

                        var semaphore = new SemaphoreSlim(1000);

                        foreach (var data in filteredDatas)
                        {
                            int dataId = data.gid.ToObject(typeof(int));

                            if (dataId == null)
                            {
                                continue;
                            }

                            //bool skipProcessing = false;

                            if (_processedLotGids.ContainsKey(dataId))
                            {
                                continue;
                            }

                            var geometry = data.geom;
                            tasks.Add(Task.Run(async () =>
                            {
                                await semaphore.WaitAsync();
                                try
                                {
                                    if (_processedLotGids.ContainsKey(dataId))
                                    {
                                        return; // Skip if already processed
                                    }

                                    if (geometry.type == "Point")
                                    {
                                        var coords = geometry.coordinates;
                                        var latLng = new LatLngLiteral(coords[1], coords[0]);
                                        await CreateMarker(latLng, data); // Assuming CreateMarker is async
                                    }
                                    else if (geometry.type == "Polygon" || geometry.type == "MultiPolygon")
                                    {
                                        IEnumerable<IEnumerable<LatLngLiteral>> latLngs = Enumerable.Empty<IEnumerable<LatLngLiteral>>();

                                        if (geometry.type == "Polygon")
                                        {
                                            var polygonCoords = geometry.coordinates[0];
                                            latLngs = new List<IEnumerable<LatLngLiteral>> { ConvertGeoJsonToLatLng(polygonCoords) };
                                        }
                                        else if (geometry.type == "MultiPolygon")
                                        {
                                            List<IEnumerable<LatLngLiteral>> multiPolygonCoords = new List<IEnumerable<LatLngLiteral>>();

                                            foreach (var polygon in geometry.coordinates)
                                            {
                                                multiPolygonCoords.Add(ConvertGeoJsonToLatLng(polygon[0]));
                                            }

                                            latLngs = multiPolygonCoords;
                                        }

                                        await CreatePolygon(latLngs, data); // Assuming CreatePolygon is async
                                    }

                                    //_drawnLots[dataId] = geometry.type;
                                    _processedLotGids[dataId] = true;
                                }
                                catch (Exception geometryEx)
                                {
                                    Console.WriteLine($"Error processing geometry for ID {data.id}: {geometryEx.Message}");
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            }));

                        }

                        if (tasks.Count > 0)
                        {
                            await Task.WhenAll(tasks);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API request error: {ex.Message}");
            }
        }

        private async Task GeneratePremisData(double southLat, double westLng, double northLat, double eastLng)
        {
            try
            {
                string requestUrl = $"/api/Premis/GetListByBound?crs=4326&minLng={westLng}&minLat={southLat}&maxLng={eastLng}&maxLat={northLat}";
                var response = await _ApiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        var tasks = new List<Task>();
                        dynamic datas = JsonConvert.DeserializeObject(dataString);

                        List<dynamic> dataList = datas.ToObject<List<dynamic>>();
                        var filteredDatas = dataList.Where(d =>
                        {
                            int dataId = (int)d.gid;  // Convert gid to int (ensure it's valid)
                            return !_processedPremisGids.ContainsKey(dataId);  // Only include items whose gid is not in _processedLotGids
                        }).ToList();

                        var semaphore = new SemaphoreSlim(1000);

                        foreach (var data in filteredDatas)
                        {
                            int dataId = data.gid.ToObject(typeof(int));

                            if (dataId == null)
                            {
                                continue;
                            }

                            //bool skipProcessing = false;

                            if (_processedPremisGids.ContainsKey(dataId))
                            {
                                continue;
                            }

                            var geometry = data.geom;
                            tasks.Add(Task.Run(async () =>
                            {
                                await semaphore.WaitAsync();
                                try
                                {
                                    if (_processedPremisGids.ContainsKey(dataId))
                                    {
                                        return; // Skip if already processed
                                    }

                                    if (geometry.type == "Point")
                                    {
                                        var coords = geometry.coordinates;
                                        double x = coords[1];
                                        double y = coords[0];
                                        var latLng = new LatLngLiteral(x, y);
                                        await CreateMarker(latLng, data); // Assuming CreateMarker is async
                                    }
                                    else if (geometry.type == "Polygon" || geometry.type == "MultiPolygon")
                                    {
                                        IEnumerable<IEnumerable<LatLngLiteral>> latLngs = Enumerable.Empty<IEnumerable<LatLngLiteral>>();

                                        if (geometry.type == "Polygon")
                                        {
                                            var polygonCoords = geometry.coordinates[0];
                                            latLngs = new List<IEnumerable<LatLngLiteral>> { ConvertGeoJsonToLatLng(polygonCoords) };
                                        }
                                        else if (geometry.type == "MultiPolygon")
                                        {
                                            List<IEnumerable<LatLngLiteral>> multiPolygonCoords = new List<IEnumerable<LatLngLiteral>>();

                                            foreach (var polygon in geometry.coordinates)
                                            {
                                                multiPolygonCoords.Add(ConvertGeoJsonToLatLng(polygon[0]));
                                            }

                                            latLngs = multiPolygonCoords;
                                        }

                                        await CreatePolygon(latLngs, data);
                                    }

                                    _processedPremisGids[dataId] = true;
                                }
                                catch (Exception geometryEx)
                                {
                                    Console.WriteLine($"Error processing geometry for ID {data.id}: {geometryEx.Message}");
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            }));

                        }

                        if (tasks.Count > 0)
                        {
                            await Task.WhenAll(tasks);

                            _markerClustering = await MarkerClustering.CreateAsync(map1.JsRuntime, map1.InteropObject, _clusteringMarkers, new()
                            {
                                ZoomOnClick = true,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API request error: {ex.Message}");
            }
        }

        public IEnumerable<LatLngLiteral> ConvertGeoJsonToLatLng(JArray geoJsonCoords)
        {
            List<List<double>> coords = geoJsonCoords.ToObject<List<List<double>>>();
            return coords.Select(coord => new LatLngLiteral(coord[1], coord[0]));
        }

        private async Task CreateMarker(LatLngLiteral position, dynamic data)
        {
            int dataId = data.gid.ToObject(typeof(int));
            string title = data.lot;
            //var marker = await Marker.CreateAsync(this.map1.JsRuntime, new MarkerOptions
            //{
            //    Position = position,
            //    Map = this.map1.InteropObject,
            //    Title = title
            //});

            var marker = await AdvancedMarkerElement.CreateAsync(this.map1.JsRuntime, new AdvancedMarkerElementOptions()
            {
                Position = position,
                Map = this.map1.InteropObject,
                Title = title,
                // Content = index.ToString()
                Content = @"<div><svg xmlns=""http://www.w3.org/2000/svg"" width=""26"" height=""26"" viewBox=""0 0 30 30"">
                    <circle cx=""15"" cy=""15"" r=""5"" fill='" + GetColorLot(1) + "'/></svg><lable class='map-marker-label'>" + $"{title}" + "</lable></div>",
            });

            _clusteringMarkers.Add(marker);

            await marker.AddListener<MouseEvent>("click", async (e) =>
            {
                await OpenSideBar(title); // Replace with actual function to handle click
                StateHasChanged();
            });
        }

        private async Task CreatePolygon(IEnumerable<IEnumerable<LatLngLiteral>> latLngs, dynamic data)
        {
            int dataId = data.gid.ToObject(typeof(int));

            var polygonOptions = new PolygonOptions
            {
                Paths = latLngs,
                StrokeColor = "#0000FF",
                StrokeOpacity = (float?)0.8,
                StrokeWeight = 2,
                FillColor = "#0000FF",
                FillOpacity = (float?)0.35
            };

            var polygon = await GoogleMapsComponents.Maps.Polygon.CreateAsync(this.map1.JsRuntime, polygonOptions);
            await polygon.SetMap(this.map1.InteropObject);
            // incase need to popup
            //await polygon.AddListener<MouseEvent>("click", async (e) =>
            //{
            //    await OpenSideBar(data.NoLesen);
            //    StateHasChanged();
            //});
        }
        #endregion
    }

    //public class FilterData
    //{


    //}
}
