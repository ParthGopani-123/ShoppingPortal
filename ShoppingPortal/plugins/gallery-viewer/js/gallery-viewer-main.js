'use strict';
if ($(".docs-image").length > 0 || $(".set-imagegallery").length > 0) {
    //var Viewer = window.Viewer;
    var console = window.console || { log: function() { } };
    var toggles = document.querySelector('.docs-toggles');
    var buttons = document.querySelector('.docs-buttons');
    var options = {
        // inline: true,
        url: 'data-original',
        build: function(e) {
            console.log(e.type);
        },
        built: function(e) {
            console.log(e.type);
        },
        show: function(e) {
            console.log(e.type);
        },
        shown: function(e) {
            console.log(e.type);
        },
        hide: function(e) {
            console.log(e.type);
        },
        hidden: function(e) {
            console.log(e.type);
        },
        view: function(e) {
            console.log(e.type);
        },
        viewed: function(e) {
            console.log(e.type);
            // this.viewer.zoomTo(1).rotateTo(180);
        }
    };

    if ($(".docs-image").length > 0)
        new Viewer(document.querySelector('.docs-image'), options);

    $(".set-imagegallery").each(function() {
        var Classname = $(this).attr("imagegalleryclass");
        if (Classname != "" && $("." + Classname).length > 0) {
            new Viewer(document.querySelector('.' + Classname + ''), options);
        }
    });
}