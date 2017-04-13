(function (module, urls) {
    'use strict';

    module.MainViewModel = function () {
        var self = this;
        self.loading = ko.observable(false);
        self.statisticsData = ko.observable(null);
        self.hasData = ko.computed(function () {
            if (self.statisticsData() && self.statisticsData().length > 0) {
                return true;
            };
            return false;
        });

        self.getStatistics = function () {
            self.loading(true);
            $.ajax({
                url: urls.getStatistics(),
                type: 'GET'
            }).done(function (response) {
                self.statisticsData(response);
            }).fail(function (e) {
                var errorMessage = 'Ошибка загрузки, попрубуйте снова.';
                //ToDo заметинь на toastr или аналогичный симпатичный уведомляльщик.
                alert(errorMessage);
            })
                .always(function () {
                    self.loading(false);
                });
        }

        self.init = function () {
            self.getStatistics();
        }

    }

})(ShortLink.Apps.UrlStatistics, ShortLink.Urls);
