/// <reference path="/ISPReferences/ISPConsole/action.htm.isp.js">
/*<!--*/
$load('ISPConsole/Frame.master.js')({
    title: 'Action:"' + $subPage.replace(/_/g, '/') + '"',
    head: function () {
    },
    body: function () {

        /*-->
        <form method="get" action="/<!--*/
        $(action.Name); /*-->">
<h3><!--*/
        $(action.Name); /*--></h3>
        <h4><!--*/
        $(action.Comment); /*--></h4>
<h4>Parameters:</h4>
<!--*/
        var parametersInfo = action.Parameters;
        var outputsInfo = action.Outputs;
        for (var i = 0; i < parametersInfo.length; i++) {
            var t = parametersInfo[i].Type;
            /*--><p><!--*/$(parametersInfo[i].Name); /*-->(<!--*/$(parametersInfo[i].Type); /*-->):<!--*/$(parametersInfo[i].Comment); /*--></p><p>&nbsp;&nbsp;<input type="<!--*/$(t == 'System.Boolean' ? 'checkbox' : (t == 'System.Web.HttpPostedFile' ? 'file' : 'text')); /*-->" value="<!--*/$(parametersInfo[i].DefaultValue); /*-->" name="<!--*/$(parametersInfo[i].Name); /*-->" /></p>
<!--*/
        } /*-->
      <input type="submit" class="btn btn-primary" value="Execute" />
<h4>Outputs:</h4>
<!--*/
        for (var i = 0; i < outputsInfo.length; i++) {
            var t = outputsInfo[i].Type;
            /*--><p><!--*/$(outputsInfo[i].Name); /*-->(<!--*/$(outputsInfo[i].Type); /*-->):<!--*/$(outputsInfo[i].Comment); /*--></p>
<!--*/
        } /*-->
      </form>
<!--*/
    }
});     //-->