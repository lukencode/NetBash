
(function ($, window) {

    var lastCommand;

    this.setError = function (message) {
        $('<div class="console-response error"/>').html(message).appendTo('#console-result');
    };

    this.scrollBottom = function () {
        $("#console-result").prop({ scrollTop: $("#console-result").prop("scrollHeight") });
    };

    this.sendCommand = function (text) {
        //update ui, todo only if not expanded
        $("#netbash-wrap").animate({
            height: '500px'
        }, 100);

        $("#console-result").fadeIn("fast");
        $("#console-input input").val("");

        $('<div class="console-request"/>').html(text).appendTo('#console-result');

        //clear command
        if (text == "clear") {
            $("#console-result").html("");
        } else {
            //send command
            $.ajax({
                url: '/netbash',
                dataType: 'json',
                data: { Command: text },

                success: function (data) {
                    if (data.Success) {
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
                },

                error: function (xhr, ajaxOptions, thrownError) {
                    setError(thrownError.toString());
                    scrollBottom();
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
                $("#console-result").fadeOut("fast");
                $("#netbash-wrap").animate({
                    height: '25px'
                }, 100);
            };
        });
    });
})(jQuery, window);