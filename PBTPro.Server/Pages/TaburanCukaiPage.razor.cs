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
using PBTPro.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PBTPro.Data;
using System.Threading;
using PBTPro.DAL.Models.PayLoads;
using System.Collections.Concurrent;
using DevExpress.Blazor;
using DevExpress.Data.Storage;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Xpo.Logger;
using PBTPro.DAL.Models.CommonServices;
using System.Reflection;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.DataAccess.Native.Web;
using NetTopologySuite.Index.HPRtree;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace PBTPro.Pages
{
    public partial class TaburanCukaiPage
    {
        bool PanelVisible { get; set; }

        //DxWindow windowRef;
        //ElementReference popupTarget;
        //bool windowVisible;

        List<taburan_view> _dtDataTaburan { get; set; }
        taburan_view _valueTaburan { get; set; }

        //For check box ===========
        public List<FilterData> FilterList { get; set; }
        protected List<string> SelectedIds = new List<string>();
        protected List<string> SelectedLots = new List<string>();
        public List<FilterData> ObjectList { get; set; }
        public string OutPutValue { get; set; }

        //////public List<FilterData> NotaList { get; set; }
        //=======================

        private string LesenID { get; set; } = string.Empty;
        private string _labelText = "";

        ////Lesen Information
        ////IEnumerable<NoticeProp> NoticeData;
        ////IEnumerable<(NoticeProp, int)> Items;

        //Premis data
        //////IEnumerable<dynamic> premisData;
        IEnumerable<general_search_premis_detail> searchData;
        //IEnumerable<(general_search_premis_detail, int)> Items;
        IEnumerable<(premis_license_tax_view, int)> premisItem;
        premis_view premisInfo;

        IGrid Grid { get; set; }
        TaskCompletionSource<bool> DataLoadedTcs { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);


        int intLegend;
        private Autocomplete autocomplete;
        private string message;
        private ElementReference searchBox;

        //Total count map based on boundries
        private int mintAktif { get; set; } = 0;
        private int mintTamatTempoh { get; set; } = 0;
        private int mintGantung { get; set; } = 0;
        private int mintTiadaData { get; set; } = 0;

        //Map
        private GoogleMap map1;
        private MapOptions _mapOptions;

        private ControlPosition _controlPosition = ControlPosition.TopLeft;
        [Inject] private IJSRuntime JsRuntime { get; set; }
        public int ZIndex { get; set; } = 0;
        private readonly Stack<Marker> markersPin = new Stack<Marker>();
        private readonly Stack<Marker> markerSearch = new Stack<Marker>();
        private readonly Stack<AdvancedMarkerElement> markers = new Stack<AdvancedMarkerElement>();

        private LatLngBounds _bounds = null!;
        private MarkerClustering? _markerClustering;
        private List<AdvancedMarkerElement>? _clusteringMarkers; // ismail changing to list - this list can be append when have new point from api

        // 07/11/2024 - to reduce API request frequency - ismail
        private DateTime _lastMoveTime;
        private readonly TimeSpan _throttleTime = TimeSpan.FromMilliseconds(300);
        private ConcurrentDictionary<int, bool> _processedLotGids = new ConcurrentDictionary<int, bool>(); // Thread-safe GID tracking
        private ConcurrentDictionary<string, bool> _processedPremisGids = new ConcurrentDictionary<string, bool>(); // Thread-safe GID tracking
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
                    return "Grey";
                default:
                    return "Black";
            }
        }

        int GetIdColor(string color)
        {
            switch (color.Trim().ToUpper())
            {
                case "GREEN":
                    return 1;
                case "RED":
                    return 2;
                case "GREY":
                    return 3;
                default:
                    return 0;
            }
        }

        int GetIdStatusColor(string status)
        {
            switch (status.Trim().ToUpper())
            {
                case "CUKAI TERTUNGGAK":
                    return 2;
                case "CUKAI DIBAYAR":
                    return 1;
                case "TIADA DATA":
                    return 3;
                default:
                    return 0;
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

        public MarkupString GetStatusIconHtml(int status_id)
        {
            string priorytyClass = "none";
            string title = " TIADA DATA ";
            if (status_id == 1)
            {
                priorytyClass = "info";
                title = " CUKAI DIBAYAR ";
            }
            else if (status_id == 2)
            {
                priorytyClass = "danger";
                title = " CUKAI TERTUNGGAK ";
            }

            string html = string.Format("<span class='e-badge e-priority-{0} py-1 px-2' title='{1}'>{1}</span>", priorytyClass, title);
            return new MarkupString(html);
        }

        //public async Task OpenNewStreetWindow(string latitude, string longitude)
        //{
        //    await JSRuntime.InvokeVoidAsync("open", "http://maps.google.com/maps?q=&layer=c&cbll=" + latitude + "," + longitude + "&cbp=11,0,0,0,0", "_blank");
        //}

        /*
        Description: Set the map properties
        Author: Azmee
        Date: November 2024
        Version: 1.0
        */
        protected override async Task OnInitializedAsync()
        {
            PanelVisible = true;

            //Show legend
            intLegend = 0;
            //Map
            _mapOptions = new MapOptions
            {
                Zoom = 16,
                ZoomControlOptions = new ZoomControlOptions
                {
                    Position = ControlPosition.RightBottom
                },
                Center = new LatLngLiteral
                {
                    Lat = 2.9441900567880896,
                    Lng = 101.37866540001413
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
            //NotaList = GetNotaFilter();
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

                ////////Get record
                //////NoticeData = await _NoticeService.GetNoticeAsync();
                //Items = NoticeData.Select((item, index) => (item, index));


                //////DataLoadedTcs.TrySetResult(true);
                //////Start populate the map
                ////await InvokeClustering(1);

                // Add listener for map api polygon data -- ismail
                await ProcessMapAPIData(1);

                ////Get record
                //premisData = await _PremisService.GetList();
                //Get search record
                searchData = await _SearchService.GetListPremisDetails();

                ////premisItem = premisData.Select((item, index) => (item, index));

                DataLoadedTcs.TrySetResult(true);
                //////await InvokeClustering(1);

                await this.map1.InteropObject.AddListener("dragend", async () =>
                {
                    await ProcessMapAPIData();
                    StateHasChanged();
                });
                await this.map1.InteropObject.AddListener("zoom_changed", async () =>
                {
                    await ProcessMapAPIData();
                    StateHasChanged();
                });

                PanelVisible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnAfterMapInit error : {ex.Message}");
            }

        }

        private async Task InvokeClustering(int initStart, double southLat, double westLng, double northLat, double eastLng)
        {
            try
            {
                //_clusteringMarkers = await populateMarker(initStart, southLat, westLng, northLat, eastLng);

                // Populate the markers list
                var newMarkers = await populateMarker(initStart, southLat, westLng, northLat, eastLng);

                // Append the new markers to the existing list
                if (_clusteringMarkers == null)
                {
                    _clusteringMarkers = newMarkers;
                }
                else
                {
                    if (_clusteringMarkers.Count == 0)
                    {
                        _clusteringMarkers = newMarkers;
                    }
                    else
                    {
                        _clusteringMarkers.AddRange(newMarkers);
                    }
                }

                if (_markerClustering != null && _clusteringMarkers != null)
                {
                    await _markerClustering.RemoveMarkers(_clusteringMarkers);
                }

                _markerClustering = await MarkerClustering.CreateAsync(map1.JsRuntime, map1.InteropObject, _clusteringMarkers, new()
                {
                    //RendererObjectName = "customRendererLib.interpolatedRenderer",
                    BatchSize = 40,
                    ZoomOnClick = true,
                });

                //////_markerClustering = await MarkerClustering.CreateAsync(map1.JsRuntime, map1.InteropObject, _clusteringMarkers, new()
                //////{
                //////    //RendererObjectName = "customRendererLib.interpolatedRenderer",
                //////    ZoomOnClick = true,
                //////});

            }
            catch (Exception ex)
            {
            }
        }

        private async Task<List<AdvancedMarkerElement>> populateMarker(int initStart, double southLat, double westLng, double northLat, double eastLng)
        {
            //var result = new List<AdvancedMarkerElement>(premisData.Count()); //AZMEE
            var result = new List<AdvancedMarkerElement>();
            try
            {
                //=========== ADD HERE =========
                mintAktif = 0;
                mintTamatTempoh = 0;
                mintTiadaData = 0;
                //==============================

                string requestUrl = $"/api/Premis/GetFilteredListByBound?crs=4326&minLng={westLng}&minLat={southLat}&maxLng={eastLng}&maxLat={northLat}";
                var response = await _ApiConnector.ProcessLocalApi(requestUrl);

                if (response.ReturnCode == 200)
                {
                    string? dataString = response?.Data?.ToString();
                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        var tasks = new List<Task>();
                        dynamic datas = JsonConvert.DeserializeObject(dataString);

                        List<dynamic> dataList = datas.ToObject<List<dynamic>>();

                        bool blnValidFilter = true;
                        string premisId = string.Empty;
                        string cukai_status = string.Empty;
                        string marker_color = string.Empty;
                        string no_lot = string.Empty;
                        int cukai_status_id = 0;

                        foreach (var _premis in dataList)
                        {
                            premisId = _premis.codeid_premis;
                            cukai_status = _premis.marker_cukai_status;
                            //marker_color = _premis.marker_color;
                            no_lot = _premis.lot.ToString();
                            cukai_status_id = GetIdStatusColor(cukai_status);
                            marker_color = GetColorLot(cukai_status_id);

                            blnValidFilter = true;
                            if (initStart != 1)
                            {
                                blnValidFilter = false;
                                if (SelectedIds.Contains(cukai_status_id.ToString()))
                                {
                                    blnValidFilter = true;
                                }
                            }

                            //Count all the cukai status on startup
                            if (cukai_status.ToUpper() == "CUKAI TERTUNGGAK")
                                mintTamatTempoh += 1;
                            else if (cukai_status.ToUpper() == "CUKAI DIBAYAR")
                                //Count total visible point based on boundries
                                mintAktif += 1;
                            else if (cukai_status.ToUpper() == "TIADA DATA")
                                //Count total visible point based on boundries
                                mintTiadaData += 1;

                            //===============================

                            //Start filtering based on selected tapisan
                            if (blnValidFilter)
                            {
                                if (!SelectedLots.Contains(no_lot))
                                {

                                    var geometry = _premis.geom;
                                    var coords = geometry.coordinates;
                                    //var latLng = new LatLngLiteral(coords[1], coords[0]);
                                    var latLng = new LatLngLiteral()
                                    {
                                        Lat = coords[1],
                                        Lng = coords[0]
                                    };

                                    //=========== ADD HERE ==========
                                    await _bounds.Extend(latLng);

                                    //Add the lots
                                    SelectedLots.Add(no_lot);

                                    var _marker = await AdvancedMarkerElement.CreateAsync(map1.JsRuntime, new AdvancedMarkerElementOptions()
                                    {
                                        Position = latLng,
                                        Map = map1.InteropObject,
                                        //Title = _premis.lot,  //comment the tooltip for faster loading
                                        Content = @"<div><svg xmlns=""http://www.w3.org/2000/svg"" width=""26"" height=""26"" viewBox=""0 0 30 30""><circle cx=""15"" cy=""15"" r=""5"" fill='" + marker_color + "'/></svg><lable class='map-marker-label'>" + $"{no_lot}" + "</lable></div>",
                                    });

                                    markers.Push(_marker);

                                    await _marker.AddListener<MouseEvent>("click", async e =>
                                    {
                                        //string markerLabelText = await marker.GetLabelText();
                                        //string _title = await _marker.GetTitle();
                                        // _events.Add("click on " + _title);
                                        await OpenSideBar(premisId);
                                        StateHasChanged();
                                        ///await e.Stop();
                                    });

                                    result.Add(_marker);
                                }
                            }

                            //Add all selected filter
                            if (initStart == 1) // first init
                            {
                                if (!SelectedIds.Contains(cukai_status_id.ToString()))
                                {
                                    SelectedIds.Add(cukai_status_id.ToString());
                                }
                            }

                        }
                    }
                }


                //====== ADD HERE =======
                //////////var boundsLiteral = await _bounds.ToJson();
                //////////await map1.InteropObject.FitBounds(boundsLiteral, OneOf.OneOf<int, Padding>.FromT0(5));
                //=======================
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        //////private async Task InvokeClustering(int initStart)
        //////{
        //////    try
        //////    {
        //////        _clusteringMarkers = await populateMarker(initStart);

        //////        _markerClustering = await MarkerClustering.CreateAsync(map1.JsRuntime, map1.InteropObject, _clusteringMarkers, new()
        //////        {
        //////            //RendererObjectName = "customRendererLib.interpolatedRenderer",
        //////            ZoomOnClick = true,
        //////        });

        //////        mintAktif = 0;
        //////        mintTamatTempoh = 0;
        //////        mintTiadaData = 0;
        //////        foreach (var _premis in premisData)
        //////        {
        //////            var geometry = _premis.geom;
        //////            if (geometry.type == "Point")
        //////            {
        //////                var coords = geometry.coordinates;
        //////                //var latLng = new LatLngLiteral(coords[1], coords[0]);
        //////                var latLng = new LatLngLiteral()
        //////                {
        //////                    Lat = coords[1],
        //////                    Lng = coords[0]
        //////                };
        //////                await _bounds.Extend(latLng);

        //////                //Count all the license status on startup
        //////                string cukai_status = _premis.tax_status_view;
        //////                if (cukai_status.ToUpper() == "CUKAI TERTUNGGAK")
        //////                    mintTamatTempoh += 1;
        //////                else if (cukai_status.ToUpper() == "CUKAI DIBAYAR")
        //////                    //Count total visible point based on boundries
        //////                    mintAktif += 1;
        //////                else if (cukai_status.ToUpper() == "TIADA DATA")
        //////                    //Count total visible point based on boundries
        //////                    mintTiadaData += 1;
        //////            }
        //////        }

        //////        var boundsLiteral = await _bounds.ToJson();
        //////        await map1.InteropObject.FitBounds(boundsLiteral, OneOf.OneOf<int, Padding>.FromT0(5));
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////    }
        //////}

        //////private async Task<List<AdvancedMarkerElement>> populateMarker(int initStart)
        //////{
        //////    var result = new List<AdvancedMarkerElement>(premisData.Count());
        //////    try
        //////    {
        //////        bool blnValidFilter = true;
        //////        foreach (var _premis in premisData)
        //////        {
        //////            string premisId = _premis.codeid_premis;
        //////            string tax_status = _premis.tax_status_view;
        //////            int tax_status_id = _premis.tax_status_id;

        //////            blnValidFilter = true;
        //////            if (initStart != 1)
        //////            {
        //////                blnValidFilter = false;
        //////                if (SelectedIds.Contains(tax_status_id.ToString()))
        //////                {
        //////                    blnValidFilter = true;
        //////                }
        //////            }

        //////            var geometry = _premis.geom;
        //////            var coords = geometry.coordinates;
        //////            //var latLng = new LatLngLiteral(coords[1], coords[0]);
        //////            var latLng = new LatLngLiteral()
        //////            {
        //////                Lat = coords[1],
        //////                Lng = coords[0]
        //////            };

        //////            //Start filtering based on selected tapisan
        //////            if (blnValidFilter)
        //////            {
        //////                var _marker = await AdvancedMarkerElement.CreateAsync(map1.JsRuntime, new AdvancedMarkerElementOptions()
        //////                {
        //////                    Position = latLng,
        //////                    Map = map1.InteropObject,
        //////                    Title = _premis.lot,
        //////                    // Content = index.ToString()
        //////                    Content = @"<div><svg xmlns=""http://www.w3.org/2000/svg"" width=""26"" height=""26"" viewBox=""0 0 30 30""><circle cx=""15"" cy=""15"" r=""5"" fill='" + GetColorLot(tax_status_id) + "'/></svg><lable class='map-marker-label'>" + $"{_premis.lot}" + "</lable></div>",
        //////                });

        //////                markers.Push(_marker);

        //////                await _marker.AddListener<MouseEvent>("click", async e =>
        //////                {
        //////                    //string markerLabelText = await marker.GetLabelText();
        //////                    //string _title = await _marker.GetTitle();
        //////                    // _events.Add("click on " + _title);
        //////                    await OpenSideBar(premisId);
        //////                    StateHasChanged();
        //////                    ///await e.Stop();
        //////                });

        //////                result.Add(_marker);
        //////            }

        //////            //Add all selected filter
        //////            if (initStart == 1) // first init
        //////            {
        //////                if (!SelectedIds.Contains(tax_status_id.ToString()))
        //////                {
        //////                    SelectedIds.Add(tax_status_id.ToString());
        //////                }
        //////            }

        //////        }
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////    }

        //////    return result;
        //////}

        private async Task ClearClustering()
        {
            if (_markerClustering == null || _clusteringMarkers == null)
            {
                return;
            }

            await _markerClustering.RemoveMarkers(_clusteringMarkers);

            //Empty the list
            // Clear all markers
            _clusteringMarkers?.Clear();
            // Reset the clustering instance
            _markerClustering?.Dispose();
            _markerClustering = null;
        }

        private async Task AddLegend()
        {
            intLegend++;

            if (intLegend == 1)
                await map1.InteropObject.AddControl(_controlPosition, LegendReference);
            else
            {
                intLegend = 0;
                await map1.InteropObject.RemoveControl(_controlPosition, LegendReference);
            }
        }

        private async Task OpenSideBar(string codeid)
        {
            try
            {
                //Populate all the value from parameter. ex:LesenID
                // await JsRuntime.InvokeVoidAsync("alert", msg);
                _labelText = codeid;

                if (_labelText.Length > 0)
                {
                    premisInfo = await _PremisService.GetPremisInfo(codeid);

                    if (premisInfo.premis_license_tax != null)
                    {
                        var taxInfo = premisInfo.premis_license_tax.First();
                    }
                }

                IJSObjectReference serverSideScripts4 = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/main.js");
                await serverSideScripts4.InvokeVoidAsync("openRightBar");

            }
            catch (Exception ex)
            {
            }
        }

        private async Task OpenFilter(int mode)
        {
            IJSObjectReference serverSideScripts1 = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/main.js");
            if (mode == 0)
            {
                await serverSideScripts1.InvokeVoidAsync("openNav");
            }
            else
            {
                await serverSideScripts1.InvokeVoidAsync("openNavSearch");
            }
        }

        private async Task CloseFilter()
        {
            IJSObjectReference serverSideScripts2 = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/main.js");
            await serverSideScripts2.InvokeVoidAsync("closeNav");
        }

        //Check Box
        protected async Task ShowSelectedValues()
        {
            PanelVisible = true;
            await ClearClustering();
            //Clear the populate lots
            SelectedLots.Clear();

            var bounds = await this.map1.InteropObject.GetBounds();
            if (bounds != null)
            {
                //Populate back the filtering
                if (SelectedIds.Count > 0)
                    await InvokeClustering(2, bounds.South, bounds.West, bounds.North, bounds.East);

                OutPutValue = string.Join(",", SelectedIds.ToArray());
            }

            PanelVisible = false;
            StateHasChanged();
        }
        private List<FilterData> GetMockFilter()
        {

            var vSubOne = new FilterData()
            {
                TypeId = 1,
                Description = "Cukai DiBayar",
                Color = "Green"
            };
            var vSubTwo = new FilterData()
            {
                TypeId = 2,
                Description = "Cukai Tertunggak",
                Color = "Red"
            };

            var vSubThree = new FilterData()
            {
                TypeId = 3,
                Description = "Tiada Data",
                Color = "Grey"
            };

            var vSubList = new List<FilterData>
            {
                vSubOne, vSubTwo, vSubThree
            };

            return vSubList;
        }

        //private List<FilterData> GetNotaFilter()
        //{
        //    var vSubSeven = new FilterData()
        //    {
        //        TypeId = 7,
        //        Description = "Nota Pemeriksaan",
        //        Color = "Purple"
        //    };

        //    var vSubList = new List<FilterData>
        //    {
        //        vSubSeven
        //    };

        //    return vSubList;
        //}

        void Grid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.SearchBoxContainer)
            {
                e.Style = "Width: 100%";
            }
        }

        //Search premise function, click from the grid
        protected async Task OnSelectedRow(object itemData)
        {
            //Remove the last marker ==========
            if (markerSearch.Any())
            {
                var lastMarker = markerSearch.Pop();
                await lastMarker.SetMap(null);
            }
            //====================================

            var item = (general_search_premis_detail)itemData;
            double premisLat = item.premis_latitude ?? 0;
            double premisLong = item.premis_longitude ?? 0;
            //LatLngLiteral LatLng = new LatLngLiteral { Lat = item.premis_latitude, Lng = item.premis_longitude };
            await PremiseLocation(new LatLngLiteral { Lat = premisLat, Lng = premisLong });
            //NavigationManager.NavigateTo("/reportnotis?nolesen=" + item.NoLesen, false);
        }

        private async Task PremiseLocation(LatLngLiteral? pos)
        {
            if (pos == null)
            {
                return;
            }

            //Close the search window
            await CloseFilter();

            LatLngLiteral premisePos = pos;
            ZIndex++;

            var marker = await Marker.CreateAsync(map1.JsRuntime, new MarkerOptions()
            {
                Position = premisePos,
                Map = map1.InteropObject,
                ZIndex = -1,
                Animation = Animation.Bounce,
                //Icon = new Icon()
                //{
                //    Url = "https://developers.google.com/maps/documentation/javascript/examples/full/images/beachflag.png"
                //}
                Icon = "images/icons/marker.png"
            });

            markerSearch.Push(marker);
            await this.map1.InteropObject.SetCenter(premisePos);
            await this.map1.InteropObject.SetZoom(17);

            //Set the animation
            if (!markerSearch.Any())
            {
                return;
            }
            var lastMarker = markerSearch.Peek();
            await lastMarker.SetAnimation(Animation.Bounce);

        }

        //async Task TogglePopupVisibilityAsync()
        //{
        //    if (windowVisible)
        //        await windowRef.CloseAsync();
        //    else
        //        await windowRef.ShowAtAsync(popupTarget);
        //}

        void Grid_CustomizeSummaryDisplayText(GridCustomizeSummaryDisplayTextEventArgs e)
        {
            if (e.Item.Name == "Custom")
                e.DisplayText = string.Format("Bil. : {0}", e.Value);
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

        private async Task ProcessMapAPIData(int intStart = 2)
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
                    if (intStart == 1)
                    {
                        await GenerateLotData(bounds.South, bounds.West, bounds.North, bounds.East);
                    }
                    //////await GeneratePremisData(bounds.South, bounds.West, bounds.North, bounds.East);
                    await InvokeClustering(intStart, bounds.South, bounds.West, bounds.North, bounds.East);
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

                                    //if (geometry.type == "Point")
                                    //{
                                    //    var coords = geometry.coordinates;
                                    //    var latLng = new LatLngLiteral(coords[1], coords[0]);
                                    //    await CreateMarker(latLng, data); // Assuming CreateMarker is async
                                    //}
                                    //else 
                                    if (geometry.type == "Polygon" || geometry.type == "MultiPolygon")
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

        //////private async Task GeneratePremisData(double southLat, double westLng, double northLat, double eastLng)
        //////{
        //////    try
        //////    {
        //////        string requestUrl = $"/api/Premis/GetFilteredListByBound?crs=4326&minLng={westLng}&minLat={southLat}&maxLng={eastLng}&maxLat={northLat}";
        //////        var response = await _ApiConnector.ProcessLocalApi(requestUrl);

        //////        if (response.ReturnCode == 200)
        //////        {
        //////            string? dataString = response?.Data?.ToString();
        //////            if (!string.IsNullOrWhiteSpace(dataString))
        //////            {
        //////                var tasks = new List<Task>();
        //////                dynamic datas = JsonConvert.DeserializeObject(dataString);

        //////                List<dynamic> dataList = datas.ToObject<List<dynamic>>();

        //////                ////var filteredDatas = dataList.Where(d =>
        //////                ////{
        //////                ////    string dataId = d.codeid_premis;  // Convert gid to int (ensure it's valid)
        //////                ////    return !_processedPremisGids.ContainsKey(dataId);  // Only include items whose gid is not in _processedLotGids
        //////                ////}).ToList();

        //////                ////var semaphore = new SemaphoreSlim(1000);


        //////                //////Count and mark the premise
        //////                ////foreach (var data in dataList)
        //////                ////{
        //////                ////    string dataId = data.codeid_premis.ToObject(typeof(string));

        //////                ////    if (dataId == null)
        //////                ////    {
        //////                ////        continue;
        //////                ////    }

        //////                ////    //bool skipProcessing = false;

        //////                ////    if (_processedPremisGids.ContainsKey(dataId))
        //////                ////    {
        //////                ////        continue;
        //////                ////    }

        //////                ////    var geometry = data.geom;
        //////                ////    tasks.Add(Task.Run(async () =>
        //////                ////    {
        //////                ////        await semaphore.WaitAsync();
        //////                ////        try
        //////                ////        {
        //////                ////            if (_processedPremisGids.ContainsKey(dataId))
        //////                ////            {
        //////                ////                return; // Skip if already processed
        //////                ////            }

        //////                ////            if (geometry.type == "Point")
        //////                ////            {
        //////                ////                var coords = geometry.coordinates;
        //////                ////                double x = coords[1];
        //////                ////                double y = coords[0];
        //////                ////                var latLng = new LatLngLiteral(x, y);
        //////                ////                await CreateMarker(latLng, data); // Assuming CreateMarker is async
        //////                ////            }
        //////                ////            else if (geometry.type == "Polygon" || geometry.type == "MultiPolygon")
        //////                ////            {
        //////                ////                IEnumerable<IEnumerable<LatLngLiteral>> latLngs = Enumerable.Empty<IEnumerable<LatLngLiteral>>();

        //////                ////                if (geometry.type == "Polygon")
        //////                ////                {
        //////                ////                    var polygonCoords = geometry.coordinates[0];
        //////                ////                    latLngs = new List<IEnumerable<LatLngLiteral>> { ConvertGeoJsonToLatLng(polygonCoords) };
        //////                ////                }
        //////                ////                else if (geometry.type == "MultiPolygon")
        //////                ////                {
        //////                ////                    List<IEnumerable<LatLngLiteral>> multiPolygonCoords = new List<IEnumerable<LatLngLiteral>>();

        //////                ////                    foreach (var polygon in geometry.coordinates)
        //////                ////                    {
        //////                ////                        multiPolygonCoords.Add(ConvertGeoJsonToLatLng(polygon[0]));
        //////                ////                    }

        //////                ////                    latLngs = multiPolygonCoords;
        //////                ////                }

        //////                ////                await CreatePolygon(latLngs, data);
        //////                ////            }

        //////                ////            _processedPremisGids[dataId] = true;
        //////                ////        }
        //////                ////        catch (Exception geometryEx)
        //////                ////        {
        //////                ////            Console.WriteLine($"Error processing geometry for ID {data.id}: {geometryEx.Message}");
        //////                ////        }
        //////                ////        finally
        //////                ////        {
        //////                ////            semaphore.Release();
        //////                ////        }
        //////                ////    }));

        //////                ////}

        //////                //Count display premis - AZMEE
        //////                mintAktif = 0;
        //////                mintTamatTempoh = 0;
        //////                mintTiadaData = 0;
        //////                foreach (var data in dataList)
        //////                {
        //////                    var geometry = data.geom;
        //////                    //string lesen_status = data.license_status_view;


        //////                    if (geometry.type == "Point")
        //////                    {
        //////                        if (data.marker_cukai_status.ToString().ToUpper() == "CUKAI TERTUNGGAK")
        //////                            mintTamatTempoh += 1;
        //////                        else if (data.marker_cukai_status.ToString().ToUpper() == "CUKAI DIBAYAR")
        //////                            //Count total visible point based on boundries
        //////                            mintAktif += 1;
        //////                        else if (data.marker_cukai_status.ToString().ToUpper() == "TIADA DATA")
        //////                            //Count total visible point based on boundries
        //////                            mintTiadaData += 1;

        //////                    }
        //////                }

        //////                ////if (tasks.Count > 0)
        //////                ////{
        //////                ////    await Task.WhenAll(tasks);

        //////                ////    _markerClustering = await MarkerClustering.CreateAsync(map1.JsRuntime, map1.InteropObject, _clusteringMarkers, new()
        //////                ////    {
        //////                ////        ZoomOnClick = true,
        //////                ////    });
        //////                ////}
        //////            }
        //////        }
        //////    }
        //////    catch (Exception ex)
        //////    {
        //////        Console.WriteLine($"API request error: {ex.Message}");
        //////    }
        //////}
        ///
        
        public IEnumerable<LatLngLiteral> ConvertGeoJsonToLatLng(JArray geoJsonCoords)
        {
            List<List<double>> coords = geoJsonCoords.ToObject<List<List<double>>>();
            return coords.Select(coord => new LatLngLiteral(coord[1], coord[0]));
        }

        //private async Task CreateMarker(LatLngLiteral position, dynamic data)
        //{
        //    string dataId = data.codeid_premis.ToObject(typeof(string));
        //    string title = data.codeid_premis.Substring(3);
        //    //var marker = await Marker.CreateAsync(this.map1.JsRuntime, new MarkerOptions
        //    //{
        //    //    Position = position,
        //    //    Map = this.map1.InteropObject,
        //    //    Title = title
        //    //});

        //    var marker = await AdvancedMarkerElement.CreateAsync(this.map1.JsRuntime, new AdvancedMarkerElementOptions()
        //    {
        //        Position = position,
        //        Map = this.map1.InteropObject,
        //        Title = title,
        //        // Content = index.ToString()
        //        Content = @"<div><svg xmlns=""http://www.w3.org/2000/svg"" width=""26"" height=""26"" viewBox=""0 0 30 30"">
        //            <circle cx=""15"" cy=""15"" r=""5"" fill='" + GetColorLot(data.marker_lesen_status) + "'/></svg><lable class='map-marker-label'>" + $"{title}" + "</lable></div>",
        //    });

        //    _clusteringMarkers.Add(marker);


        //    // Optionally, add a listener for the marker to show more information or interact with it
        //    await marker.AddListener<MouseEvent>("click", async (e) =>
        //    {
        //        //////await OpenSideBar(title); // Replace with actual function to handle click

        //        //Passing the gId - to get the click premise info
        //        await OpenSideBar(dataId.ToString()); // Replace with actual function to handle click
        //        StateHasChanged();
        //    });
        //}

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

        }
        #endregion
    }

}
