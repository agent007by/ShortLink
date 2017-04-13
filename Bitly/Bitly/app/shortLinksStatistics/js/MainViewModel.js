(function (module, urls) {
    'use strict';

    module.MainViewModel = function () {
        var self = this;

        self.statisticsData = ko.observable();
        self.hasData = ko.computed(function () {
            if (self.statisticsData() && self.statisticsData().length > 0) {
                return true;
            };
            return false;
        });
        self.getStatistics = function () {
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
                    });
        }

        self.init = function () {
            self.getStatistics();
        }

    }

})(ShortLink.Apps.UrlStatistics, ShortLink.Urls);
