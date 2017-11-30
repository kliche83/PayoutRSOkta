

FillGridSecurityModules();


function GetApiUrl(HandleData) {
    $.ajax({
        type: "POST",
        url: "AccountManager.aspx/JSONRequestApiUrlFromClient",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            HandleData(data.d);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            HandleData("Error getting server Key: " + jqXHR.responseText);
        }
    });
}


function FillGridSecurityModules() {

    GetApiUrl(function (output) {
        var APIURLResult = output;

        //debugger;
        //$.ajax({
        //    type: "POST",
        //    url: APIURLResult + "/api/RSSecurity/SecurityModuleGet/",
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    success: function (data) {
        //        debugger;
        //    },
        //    error: function (jqXHR, textStatus, errorThrown) {
        //        debugger;
        //        console.log("error " + textStatus);
        //        console.log("incoming Text " + jqXHR.responseText);
        //        alert("error: " + jqXHR.responseText);
        //    }
        //});



        $.getJSON(APIURLResult + "/api/RSSecurity/SecurityModuleGet/",
            function (data) {
                debugger;

                $('#SecurityModules').empty(); // Clear the table body.            
                if (data.length !== undefined) {// Loop through the list of products.
                    $.each(data, function (key, val) {
                        var row = '<td>' + val.Id + '</td><td>' + val.Description + '</td><td>' + val.ParentId + '</td>';
                        $("<tr>" + row + "</tr>").appendTo($('#SecurityModules'));// Append the name.
                    });
                }
            })
            .error(function (jqXHR, textStatus, errorThrown) {
                debugger;
                console.log("error " + textStatus);
                console.log("incoming Text " + jqXHR.responseText);
                alert("error: " + jqXHR.responseText);
            });
    });
}