(function (module, urls) {
    'use strict';

    module.MainViewModel = function () {
        var self = this;

        self.link = ko.observable();
        self.buttonIsCreateShortUrl = ko.observable(true);
        self.buttonText = ko.computed(function () {
            return self.buttonIsCreateShortUrl()?'Укоротить URL' : 'Скопировать';
        });
        
        self.getShortUrl = function () {

            if (self.buttonIsCreateShortUrl()) {
                $.ajax({
                        url: urls.generateNewUrl(self.link()),
                        type: 'PUT'
                    }).done(function(response) {
                        self.link(response);
                        self.buttonIsCreateShortUrl(false);
                        //выделяем уже укороченную ссылку для копирования
                        
                        var newShortLink = document.getElementById('txtLongUrl');
                        //newShortLink.select();
                        var range = document.createRange();
                        range.selectNode(newShortLink);
                        window.getSelection().addRange(range);
                        //ToDo решить проблему с выделением

                    }).fail(function(e) {
                        var errorMessage = 'Ошибка загрузки, попрубуйте снова.';
                        //ToDo заметинь на toastr или аналогичный симпатичный уведомляльщик.
                        alert(errorMessage);
                    })
                    .always(function() {
                    });
            } else {

                try {
                    // Теперь, когда мы выбрали текст ссылки, выполним команду копирования
                    var successful = document.execCommand('copy');
                    var msg = successful ? 'successful' : 'unsuccessful';
                } catch (err) {
                    alert('Произошел сбой, скопируйте ссылку вручную.');
                }
                window.getSelection().removeAllRanges();
            }
        }
    }

})(ShortLink.Apps.GenerateNewUrl, ShortLink.Urls);
