/// <reference path="/ISPReferences/ISPConsole/variables.htm.isp.js">
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
        for (var i in variables) {
            $(i + '&nbsp;=&nbsp;' + htmlEncode(String(variables[i])) + '<br />');
        }  /*-->

<!--*/
    }
});     //-->