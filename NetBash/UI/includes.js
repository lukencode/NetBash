
(function ($, window) {

    this.setError = function (message) {
        $('<div class="console-response error"/>').html(message).appendTo('#console-result');
    };

    this.sendCommand = function (text) {
        //update ui
        $("#netbash-wrap").animate({
            height: '500px'
        }, 100);

        $("#console-result").fadeIn("fast");
        $("#console-input input").val("");

        $('<div class="console-request"/>').html(text).appendTo('#console-result');

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
            },
            error: function (xhr, ajaxOptions, thrownError) {
                setError(thrownError.toString());
            }
        });
    };
        
    this.initUI = function () {
        var container = $('<div id="netbash-wrap"/>').appendTo('body');
        var controls = $('<div id="console-result"><div class="console-response">NetBash v1.0 for me on github <a href="https://github.com/lukencode/NetBash">lukencode/NetBash</a></div></div><div id="console-input"><span>></span><input type="text" placeholder="NetBash 1.0" /></div>').appendTo(container);
    };

    $(function () {
        initUI();

        //enter press
        $("#console-input input").keypress(function (event) {
            if (event.which == 13) {
                event.preventDefault();
                var text = $("#console-input input").val();

                if (text.length)
                    sendCommand(text);
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