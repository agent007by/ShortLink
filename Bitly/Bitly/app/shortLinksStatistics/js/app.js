﻿(function (module) {

    module.App.run = function () {
        module.App.Main = new module.Apps.UrlStatistics.MainViewModel();
        module.App.Main.init();
    };

})(ShortLink);