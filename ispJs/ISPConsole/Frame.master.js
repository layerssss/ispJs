/// <reference path="/ISPReferences/ISPConsole/Frame.master.js">
/*<!--*//*-->
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title><!--*/$(arguments[0].title);/*--> - ISP Engine</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <link rel="stylesheet" href="/ISPConsole/bootstrap.css">
    <script type="text/javascript" src="/ISPConsole/jquery-1.7.1.min.js"></script>
    <script src="/ISPConsole/bootstrap.js" type="text/javascript"></script>
    <!--*/
if (typeof (arguments[0].head) == 'function') {
    arguments[0].head();
}
/*-->
</head>
<body>
<div class="container">
<div id="footer">
<div class="panel">
<div class="inner">
<div class="pull-right">
<h5>
ISP Engine Version:0.1.1</h5>
<h6>
Console</h6>
</div>
<ul class="unstyled nav nav-pills">
<li class="<!--*/
$($cur == 'ISPConsole/Default.htm.isp.js' ? 'active' : ''); /*-->"><a href="/ISPConsole/"
                            title="">Summary</a></li>
                        <li class="<!--*/
$($cur == 'ISPConsole/debuggers.htm.isp.js' || $cur == 'ISPConsole/debug.htm.isp.js' ? 'active' : ''); /*-->">
                            <a href="/ISPConsole/debuggers.htm" title="">Debuggers</a></li>
                        <li class="<!--*/
$($cur == 'ISPConsole/actions.htm.isp.js' || $cur == 'ISPConsole/action.htm.isp.js' ? 'active' : ''); /*-->">
                            <a href="/ISPConsole/actions.htm" title="">Actions</a></li>
                        <li class="<!--*/
$($cur == 'ISPConsole/dependencies.htm.isp.js' ? 'active' : ''); /*-->">
                            <a href="/ISPConsole/dependencies.htm" title="">Dependencies</a></li>
                        <li class="<!--*/
$($cur == 'ISPConsole/variables.htm.isp.js' ? 'active' : ''); /*-->">
                            <a href="/ISPConsole/variables.htm" title="">Variables</a></li>
                    </ul>
                    <!-- / -->
                </div>
            </div>
        </div>
        <!-- /footer -->
    </div>
    <div class="container panel">
        <div class="inner">
            <!--*/
arguments[0].body();
/*--></div>
</div>
</body>
</html>
<!--*/
//-->