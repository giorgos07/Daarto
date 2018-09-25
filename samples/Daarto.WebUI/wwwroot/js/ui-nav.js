+function ($) {
    $(function () {
        $(document).on("click", "[ui-nav] a", function (e) {
            var $this = $(e.target);
            $this.is("a") || ($this = $this.closest("a"));
            var $active = $this.parent().siblings(".active");
            $active && $active.toggleClass("active").find("> ul:visible").slideUp(200);
            ($this.parent().hasClass("active") && $this.next().slideUp(200)) || $this.next().slideDown(200);
            $this.parent().toggleClass("active");
            $this.next().is("ul") && e.preventDefault();
        });

        $(document).on("click", "#log-out-form > a", function () {
            $("#log-out-form").submit();
        });
    });
}(jQuery);