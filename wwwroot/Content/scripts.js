window.dxDemo = window.dxDemo || {};

dxDemo.DashboardJsEvents = {
    onBeforeRender: (args) => {
        var dashboardControl = args.component;
        window.getDashboardControl = () => dashboardControl;
        dxDemo.Navigation.saveToUrl("mode", dashboardControl.isDesignMode() ? "designer" : "viewer");

        dashboardControl.isDesignMode.subscribe(function (isDesignValue) {
            dxDemo.Navigation.saveToUrl("mode", isDesignValue ? "designer" : "viewer");
        });

        var panelExtension = new DevExpress.Dashboard.DashboardPanelExtension(dashboardControl, { dashboardThumbnail: "./Content/DashboardThumbnail/{0}.png" });
        dashboardControl.registerExtension(panelExtension);
        panelExtension.allowSwitchToDesigner(false);

        if (!dashboardControl.findExtension("text-box-item-editor")) {
            dashboardControl.registerExtension(new DevExpress.Dashboard.Designer.TextBoxItemEditorExtension(dashboardControl))
        }
        /* Custom Properties Extension */
        dashboardControl.registerExtension(new ChartLineOptionsExtension(dashboardControl));
        /* Custom Item Extensions */
        dashboardControl.registerExtension(new FunnelD3CustomItem(dashboardControl));
        dashboardControl.registerExtension(new WebPageCustomItem(dashboardControl));
        dashboardControl.registerExtension(new OnlineMapCustomItem(dashboardControl));
    },
    onDashboardChanged: (args) => {
        var dashboardControl = args.component,
            dashboardId = args.dashboardId;
        if (dashboardId === "CustomItemExtensions") {
            !dashboardControl.findExtension("save-as") && dashboardControl.registerExtension(new SaveAsDashboardExtension(dashboardControl));
        } else {
            dashboardControl.unregisterExtension("save-as");
        }

        dxDemo.Sidebar && dxDemo.Sidebar.viewModel && dxDemo.Sidebar.viewModel.feedback && dxDemo.Sidebar.viewModel.feedback.init(dashboardId);
    }
}

dxDemo.Navigation = {
    replaceUrlValue: function(uri, key, value) {
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        var newParameterValue = value ? key + "=" + encodeURIComponent(value) : "";
        var newUrl;
        if(uri.match(re)) {
            var separator = !!newParameterValue ? '$1' : "";
            newUrl = uri.replace(re, separator + newParameterValue + '$2');
        }
        else if(!!newParameterValue) {
            newUrl = uri + separator + newParameterValue;
        }
        return newUrl;
    },
    saveToUrl: function (key, value) {
        var uri = location.href;
        var newUrl = this.replaceUrlValue(uri, key, value);
        if(newUrl) {
            if(newUrl.length > 2000) {
                newUrl = this.replaceUrlValue(uri, key, null);
            }
            history.replaceState({}, "", newUrl);
        }
    },
    navigate: function (baseLink) {
        window.location = baseLink + window.location.search;
        window.event.preventDefault ? window.event.preventDefault() : (window.event.returnValue = false);
        return false;
    }
};

dxDemo.Sidebar = {
    trackGAEvent: function () {
        if (window.ga && window.ga.getAll) {
            var _tracker = ga.getAll()[0];
            if (_tracker) _tracker.send('event', 'BlazorDashboardDemo',  "Open Sidebar", "v2 - toolbar");
        }
    }
};
