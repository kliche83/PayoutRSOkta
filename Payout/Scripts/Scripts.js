function ScheduleTrainerScripts() {
    
    $(document).ready(function () {
        
        $("#loadDiv").not(".keep").hide();
        $("#errorSpan").not(".keep").hide();
        $("#loadGif").hide();

        //$('#GridView1').on('click', 'input[id$=BtnDel]', function (e) {

        //    var scheduleTrainerId = e.currentTarget.parentNode.parentNode.cells['0'].innerHTML;
        //    var scheduleId = e.currentTarget.parentNode.parentNode.cells['1'].innerHTML;
        //    var trainerId = e.currentTarget.parentNode.parentNode.cells['2'].childNodes['1'].value;
        //    var startDate = e.currentTarget.parentNode.parentNode.cells['3'].childNodes['1'].value;
        //    var endDate = e.currentTarget.parentNode.parentNode.cells['4'].childNodes['1'].value;
        //    DeleteDialogMessage(scheduleTrainerId);
        //});


        $('#GridView1').on('click', 'input[id$=BtnAdd]', function (e) {

            $(this).toggleClass("menuActive");

            $("#notAdd").animate({ top: 250 }, 200, function () {
                $(".addDivCls").fadeIn();
            });

            //e.currentTarget.parentNode.parentNode.cells['4'].innerHTML


            $('#HiddenIdTextBox').val(e.currentTarget.parentNode.parentNode.cells['1'].innerHTML);
            $('#txtProgram').val(e.currentTarget.parentNode.parentNode.cells['6'].innerHTML);
            $('#txtStartDate').val(e.currentTarget.parentNode.parentNode.cells['7'].innerHTML);
            $('#txtEndDate').val(e.currentTarget.parentNode.parentNode.cells['8'].innerHTML);
            $('#txtStoreName').val(e.currentTarget.parentNode.parentNode.cells['9'].innerHTML);
            $('#txtStoreNumber').val(e.currentTarget.parentNode.parentNode.cells['10'].innerHTML);

            var city = "", state = "", owner = "", hub = "";

            if (e.currentTarget.parentNode.parentNode.cells['11'].innerHTML != "&nbsp;")
                city = e.currentTarget.parentNode.parentNode.cells['11'].innerHTML
            if (e.currentTarget.parentNode.parentNode.cells['12'].innerHTML != "&nbsp;")
                state = e.currentTarget.parentNode.parentNode.cells['12'].innerHTML
            if (e.currentTarget.parentNode.parentNode.cells['13'].innerHTML != "&nbsp;")
                owner = e.currentTarget.parentNode.parentNode.cells['13'].innerHTML


            var GridTrainDDL = e.currentTarget.parentNode.parentNode.cells['2'].childNodes['1'];
            $("#trainerDDL_Ins option").each(function () {
                if ($(this).text() == GridTrainDDL[GridTrainDDL.selectedIndex].text) {
                    $(this).attr('selected', 'selected');
                }
            });


            $('#txtCity').val(city);
            $('#txtState').val(state);
            $('#txtOwner').val(owner);

            $('#TrainStartDate_Ins').val("");
            $('#TrainEndDate_Ins').val("");

            $("#TrainStartDate_Ins").prop('disabled', false);
            $("#TrainEndDate_Ins").prop('disabled', false);
            $("#trainerDDL_Ins").prop('disabled', false);
            $("#InsertTrainerBtn").prop('disabled', true);
        })


        var prm = Sys.WebForms.PageRequestManager.getInstance();

        prm.add_beginRequest(function (source, args) {
            $("#loadDiv").show();
            $("#loadGif").show();
        });

        prm.add_endRequest(function (source, args) {
            //gridviewScroll();
            $("#loadDiv").not(".keep").hide();
            $("#errorSpan").not(".keep").hide();
            $("#loadGif").hide();
        });

        $("#gotIt").click(function () {
            $("#loadDiv").removeClass("keep");
            $("#loadDiv").hide();
            $("#errorSpan").removeClass("keep");
            $("#errorSpan").hide();
        });


        //$("#InsertTrainerBtn").click(function () {

        //    var parameters = '{ScheduleId: ' + $('#HiddenIdTextBox').val() +
        //                     ', TrainerId: ' + $('#trainerDDL :selected').val() +
        //                     ', TrainStartDate: "' + $("#TrainStartDate").val() +
        //                     '", TrainEndDate: "' + $("#TrainEndDate").val() + '"}';

        //    $.ajax({
        //        type: "POST",
        //        url: '<%= ResolveUrl("ScheduleTrainer.aspx/InsertTrainerBtn_Click") %>',
        //        data: parameters,
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        error: function (XMLHttpRequest, textStatus, errorThrown) {
        //            alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        //            DialogMessage("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown, "System Error");
        //        },
        //        success: function (msg) {
        //            switch (msg.d) {
        //                case 1:
        //                    DialogMessage("Trainer assigned successfully", "Information");
        //                    doAJAXPostBack();
        //                    break;
        //                case 0:
        //                    DialogMessage("Trainer cannot be assigned on this range of dates", "Information");
        //                    break;
        //                case -1:
        //                    DialogMessage("Please contact your administrator", "System Error");
        //                    break;
        //            }
        //        }
        //    });
        //});


        function doAJAXPostBack() {
            __doPostBack('PostbackButton', '')
        }

        $('#ClosePanelBtn').click(function () {
            $(".addDivCls").hide();
            $("#notAdd").animate({ top: 50 }, 200);
        });


        $(".addDivCls").hide();

        jQuery(function ($) {
            var focusedElementId = "";
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function (source, args) {
                var fe = document.activeElement;
                if (fe != null) {
                    focusedElementId = fe.id;
                } else {
                    focusedElementId = "";
                }
            });
            prm.add_endRequest(function (source, args) {
                $(".Date").datepicker();
                bindKeyDown();
                if (focusedElementId != "") {
                    $("#" + focusedElementId).focus();
                }

                $("input[type=text]").click(function (e) {
                    $(this).select();
                });

                $("input[type=text]").focus(function (e) {
                    $(this).select();
                });
            });



            $("#TrainStartDate_Ins").change(function () {
                var from = $("#TrainStartDate_Ins").val().split("/");
                var TrainerStartDate = new Date(from[2], from[0] - 1, from[1]);

                from = $("#TrainEndDate_Ins").val().split("/");
                var TrainerEndDate = new Date(from[2], from[0] - 1, from[1]);

                from = $("#txtStartDate").val().split("-");
                var ScheduleStartDate = new Date(from[0], from[1] - 1, from[2]);

                from = $("#txtEndDate").val().split("-");
                var ScheduleEndDate = new Date(from[0], from[1] - 1, from[2]);

                if (TrainerStartDate < ScheduleStartDate || TrainerStartDate > ScheduleEndDate) {
                    DialogMessage("Value must be in the range of schedule dates", "Information");
                    $("#TrainStartDate_Ins").val("");
                    $("#InsertTrainerBtn").prop('disabled', true);
                }
                else {
                    var value = $.trim($("#TrainEndDate_Ins").val());
                    if (value.length > 0) {
                        if (TrainerStartDate > TrainerEndDate) {
                            DialogMessage("Train start date must be lower or equal than train end date", "Information");
                            $("#TrainStartDate_Ins").val("");
                            $("#InsertTrainerBtn").prop('disabled', true);
                        }
                        else {
                            $("#InsertTrainerBtn").prop('disabled', false);
                        }

                    }
                }
            });

            $("#TrainEndDate_Ins").change(function () {
                var from = $("#TrainStartDate_Ins").val().split("/");
                var TrainerStartDate = new Date(from[2], from[0] - 1, from[1]);

                from = $("#TrainEndDate_Ins").val().split("/");
                var TrainerEndDate = new Date(from[2], from[0] - 1, from[1]);

                from = $("#txtStartDate").val().split("-");
                var ScheduleStartDate = new Date(from[0], from[1] - 1, from[2]);

                from = $("#txtEndDate").val().split("-");
                var ScheduleEndDate = new Date(from[0], from[1] - 1, from[2]);


                if (TrainerEndDate < ScheduleStartDate || TrainerEndDate > ScheduleEndDate) {
                    DialogMessage("Value must be in the range of schedule dates", "Information");
                    $("#TrainEndDate_Ins").val("");
                    $("#InsertTrainerBtn").prop('disabled', true);
                }
                else {
                    var value = $.trim($("#TrainStartDate_Ins").val());
                    if (value.length > 0) {
                        if (TrainerStartDate > TrainerEndDate) {
                            DialogMessage("Train start date must be lower or equal than train end date", "Information");
                            $("#TrainEndDate_Ins").val("");
                            $("#InsertTrainerBtn").prop('disabled', true);
                        }
                        else {
                            $("#InsertTrainerBtn").prop('disabled', false);
                        }
                    }
                }
            });

        });

        $(function () {
            $(".Date").datepicker();
        });

        $("input[type=text]").click(function (e) {
            $(this).select();
        });

        $("input[type=text]").focus(function (e) {
            $(this).select();
        });

        //$("#add").click(function () {
        //    $(this).toggleClass("menuActive");
        //    if ($("#notAdd").offset().top == 50) {
        //        $("#notAdd").animate({ top: 350 }, 300, function () {
        //            $("#addDiv").fadeIn();
        //        });
        //    }
        //    else {
        //        $("#addDiv").hide();
        //        $("#notAdd").animate({ top: 50 }, 300);
        //    }
        //});

        $("#cancelAdd").click(function () {
            $("#add").removeClass("menuActive");
            $(".addDivCls").hide();
            $("#notAdd").animate({ top: 50 }, 300);
        });

        //function DeleteRow(ScheduleTrainerId) {
        //    debugger;
        //    var parameters = '{ScheduleTrainerId: "' + ScheduleTrainerId + '"}';

        //    $.ajax({
        //        type: "POST",
        //        url: '<%= ResolveUrl("ScheduleTrainer.aspx/DeleteRecord") %>',
        //        data: parameters,
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        error: function (XMLHttpRequest, textStatus, errorThrown) {
        //            DialogMessage("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown, "System Error");
        //        },
        //        success: function (msg) {
        //            if (msg.d == 1)
        //                doAJAXPostBack();
        //            else
        //                DialogMessage("Please contact your administrator", "Error Deleting");
        //        }
        //    });
        //}

        function bindKeyDown() {
            $('input').keydown(function (e) {
                //            if(e.which==39)
                //            $(this).closest('td').next().find('input').focus();
                //            else if(e.which==37)
                //            $(this).closest('td').prev().find('input').focus();
                if (e.which == 13)
                    $(this).closest('tr').next().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
                else if (e.which == 40)
                    $(this).closest('tr').next().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
                else if (e.which == 38)
                    $(this).closest('tr').prev().find('td:eq(' + $(this).closest('td').index() + ')').find('input').focus();
            });
        }
        bindKeyDown();


        function DialogMessage(message, title, buttonType, Action) {

            var buttonsOpts = {}
            buttonsOpts["Ok"] = function () { $(this).dialog("close"); }

            $('<div></div>').dialog({
                modal: true,
                title: title,
                open: function () {
                    var markup = message;
                    $(this).html(markup);
                },
                buttons: buttonsOpts
            });
        }

        //function DeleteDialogMessage(scheduleTrainerId) {

        //    var buttonsOpts = {}
        //    buttonsOpts["Accept"] = function () { DeleteRow(scheduleTrainerId); }
        //    buttonsOpts["Cancel"] = function () { $(this).dialog("close"); }


        //    $('<div></div>').dialog({
        //        modal: true,
        //        title: "Delete Record",
        //        open: function () {
        //            var markup = "Are you sure you want to delete this record?";
        //            $(this).html(markup);
        //        },
        //        buttons: buttonsOpts
        //    });
        //}
    });
}


function CustomDialog(DialogText, autoOpen, height, width, modal, num_button, fn_apply, fn_cancel, fn_close) {
    if (num_button > 1) {
        $('<div id="CustomDialog">' + DialogText + '</div>').dialog({
            autoOpen: autoOpen,
            height: height,
            width: width,
            modal: modal,
            buttons: {
                Apply: function () {
                    fn_apply.apply();
                    $(this).dialog("close");
                },
                Cancel: function () {
                    if (fn_cancel == '')
                        $(this).dialog("close");
                    else
                        fn_cancel.apply();                        
                }
            },
            close: function () {
                if (fn_close == '')
                    $(this).dialog("close");
                else
                    fn_close.apply()
            }
        });
    }
    else {
        $('<div id="CustomDialog">' + DialogText + '</div>').dialog({
            autoOpen: autoOpen,
            height: height,
            width: width,
            modal: modal,
            buttons: {
                Apply: function () { fn_apply.apply() }
            },
            close: function () {
                if (fn_close == '')
                    $(this).dialog("close");
                else
                    fn_close.apply();
            }
        });
    }
}
