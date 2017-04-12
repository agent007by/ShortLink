(function (module) {

    module.Urls = {
        generateNewUrl: function (nativeUrl) {
            return '/api/shortUrl/new?' + $.param({
                nativeUrl: nativeUrl
            });
        },
        getStatistics: function () {
            return '/api/statistics';
        }
    };

})(ShortLink);
