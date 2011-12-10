
(function ($, window) {

    var lastCommand;
    var storageKey = "NetBash-History";
    var isOpen = false;
    var showLoader;

    var hasLocalStorage = function () {
        try {
            return 'localStorage' in window && window['localStorage'] !== null;
        } catch (e) {
            return false;
        }
    };

    var save = function () {
        if (!hasLocalStorage()) { return; }
        localStorage[storageKey] = $("#console-result").html();
    };

    var load = function () {
        if (!hasLocalStorage()) { return; }
        var html = localStorage[storageKey];
        $("#console-result").html(html);
    };

    var clearStorage = function () {
        if (!hasLocalStorage()) { return; }
        localStorage[storageKey] = null;
    };

    this.setError = function (message) {
        $('<div class="console-response error"/>').html(message).appendTo('#console-result');
    };

    this.scrollBottom = function () {
        //finish loading
        clearTimeout(showLoader);
        $("#console-input").removeClass("loading"); 

        $("#console-result").prop({ scrollTop: $("#console-result").prop("scrollHeight") });
    };

    this.openConsole = function () {
        if (isOpen)
            return;

        $("#console-result").fadeIn("fast");
        $("#netbash-wrap").animate({
            height: '500px'
        }, 100, function () {
            isOpen = true;
        });
    };

    this.closeConsole = function () {
        if (!isOpen)
            return;

        $("#console-result").fadeOut("fast");
        $("#netbash-wrap").animate({
            height: '25px'
        }, 100, function () {
            isOpen = false;
        });
    };

    this.startLoader = function () {
        showLoader = setTimeout("$('#console-input').addClass('loading')", 300);
    };

    this.sendCommand = function (text) {
        openConsole();
        $("#console-input input").val("");

        $('<div class="console-request"/>').html(text).appendTo('#console-result');
        scrollBottom();

        //clear command
        if (text == "clear") {
            $("#console-result").html("");
            clearStorage();
        } else {
            startLoader();

            //send command
            $.ajax({
                url: '/netbash',
                dataType: 'json',
                data: { Command: text },

                success: function (data) {
                    if (data.Success === true) {
                        if (data.IsRaw) {
                            //pre that shit
                            $('<pre class="console-response"/>').html(data.Content).appendTo('#console-result');
                        } else {
                            //regs div bro
                            $('<div class="console-response"/>').html(data.Content).appendTo('#console-result');
                        }
                    } else {
                        setError(data.Content);
                    }

                    scrollBottom();
                    save();
                },

                error: function (xhr, ajaxOptions, thrownError) {
                    setError(thrownError.toString());
                    scrollBottom();
                    save();
                }
            });
        }
    };

    this.initUI = function () {
        var container = $('<div id="netbash-wrap"/>').appendTo('body');
        var controls = $('<div id="console-result"><div class="console-message">NetBash v1.0 for me on github <a href="https://github.com/lukencode/NetBash">lukencode/NetBash</a></div></div><div id="console-input"><span>></span><input type="text" placeholder="NetBash 1.0" /></div>').appendTo(container);
    };

    $(function () {
        initUI();
        load();

        //enter press
        $("#console-input input").keyup(function (event) {
            if (event.which == 13) {
                event.preventDefault();

                var text = $("#console-input input").val();
                lastCommand = text;

                if (text.length)
                    sendCommand(text);
            } else if (event.which == 38) {
                $("#console-input input").val(lastCommand);
            }
        });

        //close
        $('html').click(function (event) {
            if (!$(event.target).closest('#netbash-wrap').length) {
                closeConsole();
            };
        });
    });
})(jQuery, window);