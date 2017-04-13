(function (module, urls) {
    'use strict';

    module.MainViewModel = function () {
        var self = this;

        self.link = ko.observable();
        self.copiedLink = ko.observable(null);
        self.buttonIsCreateShortUrl = ko.observable(true);
        self.IsEmptyLinkInput = ko.computed(function () {
            if (!self.link()) {
                self.buttonIsCreateShortUrl(true);
                return true;
            };
            return false;
        });

        self.buttonText = ko.computed(function () {
            return self.buttonIsCreateShortUrl() ? 'Укоротить URL' : 'Скопировать';
        });

        self.getShortUrl = function () {

            if (self.buttonIsCreateShortUrl()) {
                $.ajax({
                    url: urls.generateNewUrl(self.link()),
                    type: 'PUT'
                }).done(function (response) {
                    self.link(response);
                    self.buttonIsCreateShortUrl(false);
                    //выделяем уже укороченную ссылку для копирования
                    //ToDo Проверка на кроссбраузерность 
                    var newShortLink = document.getElementById('txtLongUrl');
                    var range = document.createRange();
                    range.selectNode(newShortLink);
                    var selection = window.getSelection();
                    selection.addRange(range);
                }).fail(function (e) {
                    var errorMessage = 'Ошибка загрузки, попрубуйте снова.';
                    //ToDo заметинь на toastr или аналогичный симпатичный уведомляльщик.
                    alert(errorMessage);
                })
                    .always(function () {
                    });
            } else {

                try {
                    // Теперь, когда мы выбрали текст ссылки, выполним команду копирования
                    var successful = document.execCommand('copy');
                    if (successful) {
                        self.copiedLink(self.link());
                        self.link('');
                        self.buttonIsCreateShortUrl(true);
                    }
                } catch (err) {
                    alert('Произошел сбой, скопируйте ссылку вручную.');
                }

                window.getSelection().removeAllRanges();
            }
        }
    }

})(ShortLink.Apps.GenerateNewUrl, ShortLink.Urls);
