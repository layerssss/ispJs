/// <reference path="/ISPReferences/ISPConsole/dependencies.htm.isp.js">
/*<!--*/
$load('ISPConsole/Frame.master.js')({
    title: 'Dependencies',
    head: function () {

    },
    body: function () {
        for (var i in dependencies) {
            if (typeof (dependencies[i]) != 'function') {
                if (i.substring(0, "ISPConsole".length) == "ISPConsole") { continue; }
                /*-->
                <h3><!--*/
                $(i); /*--></h3>
<!--*/
                for (var j = 0; j < dependencies[i].length; j++) {
                    var f = dependencies[i][j];
                    /*-->
                    <!--*/
                    $(f); /*-->&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Version&nbsp;:&nbsp;<!--*/$(version[f]); /*-->&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Rendered&nbsp;:&nbsp;<!--*/$(rendered[f]); /*--><br />
<!--*/
                } /*-->
<hr />

<!--*/
            }
        }
    }
});  //-->