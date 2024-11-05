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
using PBT.Data;
using DevExpress.DashboardCommon;


namespace PBT.Pages
{
    public partial class ZonMajlis 
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
        //Autocomplete
        //private MapOptions mapOptions;
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
        private IEnumerable<AdvancedMarkerElement>? _clusteringMarkers;

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
        //protected override void OnInitialized()
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
            //Start popuate the map
            await InvokeClustering(1);

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

        private async Task<IEnumerable<AdvancedMarkerElement>> populateMarker(int initStart)
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
                        ////await e.Stop();
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

            // IJSObjectReference serverSideScripts3 = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/mainPanel.js");
            // await serverSideScripts3.InvokeVoidAsync("mainPanel");

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

    }

    public class FilterData
    {
        public int TypeId
        { get; set; }
        public string Description
        { get; set; }

        public string Color
        { get; set; }

        public string ColorDesc
        {
            get
            {
                return Description + ":" + Color;
            }
        }

    }
}
