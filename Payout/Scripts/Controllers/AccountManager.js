
$(document).ready(function () {

    var APIURLResult;    
    var GridName = "";
    var RowId = "";
    var IsOktaFlag = false;

    var colName, colValue;

    setDefaultTabs();
    CleanControls();

    if (!isPostBack()) {

    }

    function CleanControls() {

    }

    function setDefaultTabs() {
        var defaultSelectedTab = "tab1"

        $('.Content-Wrapper').hide();
        $('.Content-Wrapper.' + defaultSelectedTab).show();

        $('ul li.' + defaultSelectedTab).addClass("selected");
    }

    function isPostBack() {
        return document.referrer.indexOf(document.location.href) > -1;
    }

    $('#tabsAccManager').find('li').click(function (e) {

        $('.Content-Wrapper.' + this.className).siblings(':not(ul)').hide();
        $('.Content-Wrapper.' + this.className).show();

        $(this).siblings().removeClass("selected");
        $(this).addClass("selected");
    });


    /*CREATE USERS*/
    $("#SubmitUser").click(function (event) {

        if (APIURLResult == "" || APIURLResult == undefined) {
            GetApiUrl(function (output) {
                APIURLResult = output;
                SubmitAction("CreateUser")
            });
        }
        else {
            SubmitAction("CreateUser");
        }
    });
    
    ///*UPDATE USERS*/
    $("table").find('input[type="text"]').focusout(function (e) {        
        UpdateUserAction(e, $(this)[0]);        
    });

    $("table").find('input[type="checkbox"]').change(function (e) {
        UpdateUserAction(e, $(this)[0]);
    });

    function UpdateUserAction(e, control)
    {
        if (e.target.defaultValue != e.target.value) {
            RowId = control.parentNode.parentNode.cells[0].childNodes[1].innerText;

            var arrColName = e.target.name.split('$');
            colName = arrColName[arrColName.length - 1];
            GridName = arrColName[0];
            if (e.target.attributes['2'].nodeValue == "checkbox") {
                colValue = e.target.checked;
                var DialogText = 'This action will be perform changes on Okta groups for this user. Do you want to proceed?';
                CustomDialog(DialogText, true, 240, 300, true, 2, AcceptIsOktaUpdate, '', '');
            }
            else {
                colValue = e.target.value;

                if (APIURLResult == "" || APIURLResult == undefined) {
                    GetApiUrl(function (output) {
                        APIURLResult = output;
                        submitUpdates();
                    });
                }
                else {
                    submitUpdates();
                }
            }

            e.target.defaultValue = e.target.value;
        }
    }


    function AcceptIsOktaUpdate()
    {
        
        if (APIURLResult == "" || APIURLResult == undefined) {
            GetApiUrl(function (output) {
                APIURLResult = output;
                submitUpdates();
            });
        }
    }

    function submitUpdates()
    {
        if (GridName == "GRVUserList")
            SubmitAction("UpdateUser", RowId)

        if (GridName == "GRVRoleList")
            SubmitAction("UpdateRole", RowId)
    }

    /*DELETE USERS*/
    $(".delUserLink").click(function () {        
        //var RowId = $(this)[0].parentNode.parentNode.cells[0].innerText;
        RowId = $(this)[0].parentNode.parentNode.cells[0].childNodes[1].innerText;
        
        if (APIURLResult == "" || APIURLResult == undefined) {
            GetApiUrl(function (output) {
                APIURLResult = output;
                SubmitAction("DeleteUser", RowId)
            });
        }
        else {
            SubmitAction("DeleteUser", RowId);
        }
    });

    /*CREATE ROLE*/
    $("#SubmitRole").click(function (event) {
        if (APIURLResult == "" || APIURLResult == undefined) {
            GetApiUrl(function (output) {
                APIURLResult = output;
                SubmitAction("CreateRole")
            });
        }
        else {
            SubmitAction("CreateRole");
        }
    });

    /*UPDATE ROLES*/
    function UpdateRole () {

        //var RowId = $(this)[0].parentNode.parentNode.cells[0].innerText;
        RowId = $(this)[0].parentNode.parentNode.cells[0].childNodes[1].innerText;

        if (APIURLResult == "" || APIURLResult == undefined) {
            GetApiUrl(function (output) {
                APIURLResult = output;
                SubmitAction("UpdateRole", RowId)
            });
        }
        else {
            SubmitAction("UpdateRole", RowId);
        }
    }

    /*DELETE ROLES*/
    $(".delRoleLink").click(function () {

        //var RowId = $(this)[0].parentNode.parentNode.cells[0].innerText;
        RowId = $(this)[0].parentNode.parentNode.cells[0].childNodes[1].innerText;

        if (APIURLResult == "" || APIURLResult == undefined) {
            GetApiUrl(function (output) {
                APIURLResult = output;
                SubmitAction("DeleteRole", RowId)
            });
        }
        else {
            SubmitAction("DeleteRole", RowId);
        }
    });

    //Set Roles to users
    $("table").find('select').change(function (e) {

        RowId = $(this)[0].parentNode.parentNode.cells[0].childNodes[1].innerText;
        
        var arrColName = e.target.name.split('$');
        colName = "Role";
        colValue = e.target.value;

        IsOktaFlag = $(this).parent().parent().find("[id*=IsOkta]")[0].checked;

        if (APIURLResult == "" || APIURLResult == undefined) {
            GetApiUrl(function (output) {
                APIURLResult = output;

                if (arrColName[0] == "GRVUserList")
                    SubmitAction("UpdateUserRole", RowId)
            });
        }
        else {
            if (arrColName[0] == "GRVUserList")
                SubmitAction("UpdateUserRole", RowId);
        }
    });

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

    function SubmitAction(Action, Id) {
        if (APIURLResult.indexOf("Error") >= 0) {
            alert(APIURLResult)
        }
        else {
            switch (Action) {
                case "CreateUser":
                    CreateUser(APIURLResult, $('#fname').val(), $('#lname').val(), $('#email').val(), $('#isokta')[0].checked, function (output) {
                        if (output.succeeded)
                            alert("User created successfully");
                        else
                            alert("Error creating user");
                        window.location.href = "AccountManager.aspx"
                    });
                    break;
                case "UpdateUser":
                    UpdateUser(APIURLResult, Id, colName, colValue, function (output) {
                        if (!output.succeeded)
                        {
                            alert("Error updating user");
                            window.location.href = "AccountManager.aspx";                            
                        }

                        if (colName == "IsOkta")
                            window.location.href = "AccountManager.aspx";
                    });
                    break;
                case "DeleteUser":
                    DeleteUser(APIURLResult, Id, function (output) {
                        if (output.succeeded)
                            alert("User deleted successfully");
                        else
                            alert("Error deleting user");
                        window.location.href = "AccountManager.aspx";
                    });
                    break;                
                case "CreateRole":
                    CreateRole(APIURLResult, $('#rname').val(), function (output) {
                        if (output.succeeded)
                            alert("Role created successfully");
                        else
                            alert("Error creating role");
                        window.location.href = "AccountManager.aspx"
                    });
                    break;
                case "UpdateRole":
                    UpdateRole(APIURLResult, Id, colName, colValue, function (output) {
                        if (!output.succeeded)
                            alert("Error updating role");
                        window.location.href = "AccountManager.aspx";
                    });                    
                    break;
                case "DeleteRole":
                    DeleteRole(APIURLResult, Id, function (output) {
                        if (output.succeeded)
                            alert("Role deleted successfully");
                        else
                            alert("Error deleting role");
                        window.location.href = "AccountManager.aspx";
                    });
                    break;
                case "UpdateUserRole":
                    UpdateUserRole(APIURLResult, Id, colValue, function (output) {
                        if (!output.succeeded)
                        {
                            alert("Error setting user to role");
                            window.location.href = "AccountManager.aspx";
                        }
                    });
                    break;
            }
        }
    }

    function CreateUser(APIServerURL, firstName, lastName, email, isokta, HandleData) {
        var model = { FirstName: firstName, LastName: lastName, Email: email, IsOkta: isokta };
        $.ajax({
            url: APIServerURL + "/api/account/CreateUser/",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                HandleData(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                HandleData(jqXHR.responseText);
            }
        });
    }

    function UpdateUser(APIServerURL, Id, ColName, ColValue, HandleData) {
        
        var model = { id: Id, colName: ColName, colValue: ColValue };        
        $.ajax({
            url: APIServerURL + "/api/account/UpdateUser/",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                HandleData(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                HandleData(jqXHR.responseText);
            }
        });
    }

    function DeleteUser(APIServerURL, email, HandleData) {
        
        var model = { Email: email };        
        $.ajax({
            url: APIServerURL + "/api/account/DeleteUser/",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                HandleData(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {                
                HandleData(jqXHR.responseText);
            }
        });
    }

    function CreateRole(APIServerURL, roleName, HandleData) {
        var model = { Name: roleName };
        $.ajax({
            url: APIServerURL + "/api/account/CreateRole/",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                HandleData(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                HandleData(jqXHR.responseText);
            }
        });
    }

    function UpdateRole(APIServerURL, Id, ColName, ColValue, HandleData) {        
        var model = { id: Id, colName: ColName, colValue: ColValue };                
        $.ajax({
            url: APIServerURL + "/api/account/UpdateRole/",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                HandleData(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                HandleData(jqXHR.responseText);
            }
        });
    }

    function DeleteRole(APIServerURL, name, HandleData) {
        var model = { Name: name };
        $.ajax({
            url: APIServerURL + "/api/account/DeleteRole/",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                HandleData(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                HandleData(jqXHR.responseText);
            }
        });
    }
        
    function UpdateUserRole(APIServerURL, Id, ColValue, HandleData) {
        var model = { Username: Id, Role: ColValue, IsOkta: IsOktaFlag };  
        $.ajax({
            url: APIServerURL + "/api/account/SetUserRole/",
            type: "POST",
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                HandleData(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                HandleData(jqXHR.responseText);
            }
        });
    }
});