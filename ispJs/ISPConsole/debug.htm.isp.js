/// <reference path="/ISPReferences/ISPConsole/debug.htm.isp.js">
/*<!--*/
$load('ISPConsole/Frame.master.js')({
    title: 'Debugger for "' + $subPage.replace(/_/g, '/') + '"',
    head: function () {
        /*-->
        <script type="text/javascript" language="javascript">
        var file = "<!--*/
        $($subPage.replace(/_/g, '/')); /*-->";
    var lstSrc = "";
    var timer = null;
    var refresh = function () {
        $.ajax({
            url: "/ISPDebug/",
            dataType: 'json',
            data: {
                file: file,
                command: ''
            },
            converters: {
                "text json": function (txt) {
                    return eval("a=" + txt);
                }
            },
            success: function (d, s, x) {
                if (lstSrc == x.responseText) {
                    timer = setTimeout(refresh, 1000);
                    return;
                }
                lstSrc = x.responseText;
                init();
                d = d.status;
                if (d.CurrentPosition == -1) {
                    $('#status').text('Running').attr('class', 'alert alert-success');
                }
                else {
                    $('#status').text('Paused').attr('class', 'alert alert-info');
                }
                for (var i in d.Locals) {
                    var l = d.Locals[i];
                    if (l.ShortValue == null) {
                        l.ShortValue = 'null';
                    }
                    $('<tr></tr>')
                    .append($('<td class="name"><span class="">' + l.Name + '</span></td>'))
                    .append($('<td class="value"></td>').text(l.ShortValue).attr('data-content', l.LongValue).popover({ title: l.Name, placement: 'left' }).css("color", l.Value == null ? '#ccc' : '#000'))
                    .appendTo('#locals tbody');
                }
                for (var i in d.SystemLocals) {
                    var l = d.SystemLocals[i];
                    if (l.ShortValue == null) {
                        l.ShortValue = 'null';
                    }
                    $('<tr class=""></tr>')
                    .append($('<td class="name"><span class="" style="color:#ccc">' + l.Name + '</span></td>'))
                    .append($('<td class="value"></td>').text(l.ShortValue).attr('data-content', l.LongValue).popover({ title: l.Name, placement: 'left' }).css("color", l.Value == null ? '#ccc' : '#000'))
                    .appendTo('#locals tbody');
                }
                var lstp = 0;
                $('#src').html('');
                var dpmnHtml = '<ul class="dropdown-menu"><li><a class="normal" href="#">Disable</a></li><li><a class="btn-primary" href="#">Enable</a></li><li><a class="btn-info" href="#">Enable with condition</a></li></ul>';
                for (var i in d.BreakPoints) {
                    var b = d.BreakPoints[i];
                    $('#src').append(HTMLEncode(d.Source.substring(lstp, b.Position)));
                    var bE = $('<span class="btn-group" style="display:inline;"><a class="btn dropdown-toggle" data-toggle="dropdown" href="#">&nbsp;<span class="caret"></span></a>' + dpmnHtml + '</span>')
                            .appendTo('#src');
                    switch (b.Condition) {
                        case null:
                            break;
                        case 'true;':
                            bE.children('a').addClass('btn-primary');
                            break;
                        default:
                            bE.children('a').addClass('btn-info');
                            break;
                    }
                    bE.data('bpnum', i);
                    bE.data('bpcon', b.Condition);
                    lstp = b.Position;
                    bE.find('.dropdown-menu .normal').click(function () {
                        $.getJSON('/ISPDebug/', {
                            file: file,
                            command: 'debug().SetBreakPointCondition(' + $(this).closest('.btn-group').data('bpnum') + ',null);'
                        });
                        return false;
                    });
                    bE.find('.dropdown-menu .btn-primary').click(function () {
                        $.getJSON('/ISPDebug/', {
                            file: file,
                            command: 'debug().SetBreakPointCondition(' + $(this).closest('.btn-group').data('bpnum') + ',"true;");'
                        });
                        return false;
                    });
                    bE.find('.dropdown-menu .btn-info').click(function () {
                        var str = prompt("Condition for the breakpoint:", $(this).closest('.btn-group').data('bpcon'));
                        if (str != null) {
                            $.getJSON('/ISPDebug/', {
                                file: file,
                                command: 'debug().SetBreakPointCondition(' + $(this).closest('.btn-group').data('bpnum') + ',"'+str+'");'
                            });
                        }
                        return false;
                    });
                    if (b.Position == d.CurrentPosition) {
                        $('<a href="#" class="btn btn-success">Resume</a>').appendTo('#src').click(function () {
                            $.getJSON('/ISPDebug/', {
                                file: file,
                                command: 'debug().Resume();'
                            });
                            return false;
                        });
                    }
                }

                $('#src').append(HTMLEncode(d.Source.substring(lstp)));
                timer = setTimeout(refresh, 400);
            },
            error: function (e, xhr, a, t) {
                if (e.responseText != "") {
                    init();
                    $('#status').text(e.responseText).attr('class', 'alert alert-error');
                    lstSrc = "";
                    timer = setTimeout(refresh, 500);
                } else {
                    timer = setTimeout(refresh, 1000);
                }
            }
        });
    };
    function HTMLEncode(str) {
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&/g, "&gt;");
        s = s.replace(/</g, "&lt;");
        s = s.replace(/>/g, "&gt;");
        s = s.replace(/    /g, "&nbsp;");
        s = s.replace(/\'/g, "&#39;");
        s = s.replace(/\"/g, "&quot;");
        s = s.replace(/\n/g, "<br>");
        return s;
    }
    var init = function () {
        $('#locals tbody tr').remove();
        $('#src').text('No source available...');
    };
    $(function () {
        file = $('#ctl>input').attr('readonly', true).val();
        $('#ctl .btn-primary').click(function () {
            $('#ctl>.btn').toggle();
            $('#ctl>input').attr('readonly', false);
            return false;
        });
        $('#ctl .btn-success').hide().click(function () {
            $('#ctl>.btn').toggle();
            file = $('#ctl input').attr('readonly', true).val();
            return false;
        });
        $('#ctl .btn-danger').hide().click(function () {
            $('#ctl>input').attr('readonly', true).val(file);
            $('#ctl>.btn').toggle();
            return false;
        });
        refresh();
        window.onscroll = function () {
            $('.sidebar').css({
                'right': 0,
                top: window.document.body.scrollTop,
                height: $(window).height() - 32
            });
        };
        window.onresize = window.onscroll;
        window.onscroll();
    });
</script>
<!--*/
    },
    body: function () {
        /*-->
        <div class="well" id="ctl">
        Source:<input type="text" name="file" style="width:300px;" value="<!--*/
        $($subPage.replace(/_/g, '/')); /*-->" class="span3" />
    <!--<button type="btn" class="btn-primary btn">
        Edit</button>
    <button type="btn" class="btn-success btn">
        Ok</button>
    <button class="btn-danger btn">
        Cancle</button>-->
    &nbsp;&nbsp;&nbsp;Status:<span class="alert alert-error" id="status">
    </span>
    </form>
    <pre id="src" class="btn-toolbar source-code">

                </pre>
</div>
<div class="sidebar panel">
    <!--Sidebar content-->
    <h3>
        Locals:</h3>
    <table id="locals" class="locals table table-bordered table-condensed table-striped">
        <thead>
            <tr>
                <th>
                    Name
                </th>
                <th>
                    Value
                </th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</div>
<!--*/
    }
});
//-->