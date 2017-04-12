(function(module) {
    'use strict';

    $(function() {
        module.App.run && module.App.run();
        if (module.App.Main) {
            ko.applyBindings(module.App.Main, document.getElementById('pageContent'));
        }
    });

})(ShortLink);
