/// <reference path="/ISPReferences/ISPConsole/actions.htm.isp.js">
/*<!--*/
$load('ISPConsole/Frame.master.js')({
    title: 'Actions',
    head: function () {

    },
    body: function () {
        var text = JSON.stringify(subPages);

        /*-->
        <h3>Actions:</h3>
        <!--*/
        for (var i = 0; i < actions.length; i++) {
            /*--><a href="<!--*/$(actions[i].Name.replace(/\//g, '_')); /*-->.action.htm"><!--*/$(actions[i].Name); /*-->:<!--*/$(actions[i].Comment); /*--></a><br /><!--*/
        } /*-->

<!--*/
    }
}); //-->