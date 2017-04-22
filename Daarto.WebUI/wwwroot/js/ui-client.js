+function ($) {
    $(function () {
        var isIe = !!navigator.userAgent.match(/MSIE/i) || !!navigator.userAgent.match(/Trident.*rv:11\./);
        isIe && $("html").addClass("ie");
        var userAgent = window["navigator"]["userAgent"] || window["navigator"]["vendor"] || window["opera"];
        (/iPhone|iPod|iPad|Silk|Android|BlackBerry|Opera Mini|IEMobile/).test(userAgent) && $("html").addClass("smart");
    });
}(jQuery);