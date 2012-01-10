
function NetBash($, window, opt) {

    var options = opt || {};

    var self = this;
    var lastCommand;
    var storageKey = "NetBash-History";
    var hiddenKey = "NetBash-Hidden";
    var commandKey = "NetBash-LastCommand";
    var isOpen = false;
    var isHidden = (options.isHidden === true);
    var showLoader;
    var existingHtml;

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
        localStorage[hiddenKey] = isHidden;
        localStorage[commandKey] = lastCommand;
    };

    var load = function () {
        if (!hasLocalStorage()) { return; }

        existingHtml = localStorage[storageKey];

        var localStorageHidden = localStorage[hiddenKey];
        if (localStorageHidden != null)
            isHidden = (localStorageHidden == 'true');

        lastCommand = localStorage[commandKey];
    };

    var clearStorage = function () {
        if (!hasLocalStorage()) { return; }
        localStorage[storageKey] = null;
    };

    this.setError = function (message) {
        $('<div class="console-error"/>').html(message).appendTo('#console-result');
    };

    this.scrollBottom = function () {
        //finish loading
        clearTimeout(showLoader);
        $("#console-input").removeClass("loading");

        $("#console-result").scrollTop($("#console-result")[0].scrollHeight);
    };

    this.openConsole = function () {
        if (isOpen)
            return;

        $("#console-input input").focus();

        $("#console-result").fadeIn("fast");
        $("#netbash-wrap").animate({
            height: '500px'
        }, 100, function () {
            isOpen = true;
        });
        self.scrollBottom();
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

        $("#console-input input").blur();
    };

    this.startLoader = function () {
        showLoader = setTimeout("$('#console-input').addClass('loading')", 300);
    };

    this.sendCommand = function (text) {
        self.openConsole();
        $("#console-input input").val("");

        $('<div class="console-request"/>').html(text).appendTo('#console-result');
        self.scrollBottom();

        //clear command
        if (text == "clear") {
            $("#console-result").html("");
            clearStorage();
        } else {
            self.startLoader();

            //send command
            $.ajax({
                url: options.routeBasePath + 'netbash',
                dataType: 'json',
                data: { Command: text },

                success: function (data) {
                    if (data.Success === true) {
                        if (data.IsHtml) {
                            //regs div bro
                            $('<div class="console-response"/>').html(data.Content).appendTo('#console-result');
                        } else {
                            //pre that shit
                            $('<pre class="console-response">' + data.Content + '</pre>').appendTo('#console-result');
                        }
                    } else {
                        self.setError(data.Content);
                    }

                    self.scrollBottom();
                    save();
                },

                error: function (xhr, ajaxOptions, thrownError) {
                    self.setError(thrownError.toString());
                    self.scrollBottom();
                    save();
                }
            });
        }
    };

    this.initUI = function () {
        var container = null;

        if (isHidden) {
            container = $('<div id="netbash-wrap" style="display:none;"/>').appendTo('body');
        } else {
            container = $('<div id="netbash-wrap"/>').appendTo('body');
        }

        var controls = $('<div id="console-result"><div class="console-message">' + options.welcomeMessage + '</div></div><div id="console-input"><span>></span><input type="text" placeholder="NetBash ' + options.version + ' " /></div>').appendTo(container);

        if (existingHtml) {
            $("#console-result").html(existingHtml);
        }
    };

    this.toggleConsole = function () {
        $('#netbash-wrap').slideToggle(90, function () {
            isHidden = $('#netbash-wrap').is(":hidden");
            localStorage[hiddenKey] = isHidden;
        });
    };

    $(function () {
        load();
        self.initUI();

        //enter press
        $("#console-input input").keyup(function (event) {
            if (event.which == 13) { //enter
                event.preventDefault();

                var text = $("#console-input input").val();
                lastCommand = text;

                if (text.length) {
                    self.sendCommand(text);
                } else {
                    self.openConsole();
                }
            } else if (event.which == 38) { //up
                $("#console-input input").val(lastCommand);
            } else if (event.which == 27) { // || event.which == 192) { //escape or `
                self.closeConsole();
            } else if (event.which == 192 && $("#console-input input").val().indexOf("~") != -1) {
                self.closeConsole();
                self.toggleConsole();
                $("#console-input input").val("");
            }
        });

        //close
        $('html').click(function (event) {
            if (!$(event.target).closest('#netbash-wrap').length) {
                self.closeConsole();
            };
        });

        //bind keyboard shortcuts
        key('`', function () {
            self.openConsole();

            if (isHidden)
                self.toggleConsole();

            return false;
        });

        key('shift+`', function () {
            self.closeConsole();
            self.toggleConsole();
        });

        //bind keyboard shortcuts
        key('esc', function () {
            self.closeConsole();
        });

    });
}