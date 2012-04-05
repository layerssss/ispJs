/// <reference path="/ISPReferences/ISPConsole/Default.htm.isp.js">
/*<!--*/

$load('ISPConsole/Frame.master.js')({
    title: 'Summary',
    head: function () {
    
    },
    body: function () {

        /*-->
        <h3>SubPages:</h3>
        <!--*/
        for (var i = 0; i < subPages.length; i++) { $(subPages[i].Folder); /*-->*.<!--*/$(subPages[i].Extension); /*--><br /><!--*/ } /*-->

<hr />
<h3>Renderers:</h3>
<!--*/
        for (var i = 0; i < renderers.length; i++) { $(renderers[i]); /*--><br /><!--*/ } /*-->
<hr />
<h3>Actions:</h3>
<!--*/

        $(actions.length);  /*--> registered.
        
<!--*/
    }
});
//-->