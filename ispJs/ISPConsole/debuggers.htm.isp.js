/// <reference path="/ISPReferences/ISPConsole/debuggers.htm.isp.js">
/*<!--*/
$load('ISPConsole/Frame.master.js')({
    title: 'Debuggers',
    head: function () {
    },
    body: function () {
        var text = JSON.stringify(subPages);

        /*-->
        <h3>Debuggers:</h3>
        <!--*/
        for (var i in debuggers) {
            if (debuggers[i].CurrentPosition != undefined) {
                if (i.substring(0, "ISPConsole".length) == "ISPConsole") { continue; }
                /*--><a href="<!--*/$(i.replace(/\//g, '_')); /*-->.debug.htm"><!--*/$(i); /*--> : <!--*/
                if (debuggers[i].CurrentPosition == -1) {
                    $('Running');
                }
                else {
                    $('Paused');
                }
                /*--></a><br /><!--*/
            } 
        }
    }
});
    
    //-->