<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Howto.aspx.cs" Inherits="Payout.Howto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payouts | How-to</title>
    <link href="Content/Style.css" rel="stylesheet" type="text/css" />
    <%--<script src="http://cdn.jquerytools.org/1.2.7/full/jquery.tools.min.js"></script>--%>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.0.3/jquery.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".page").click(function goTo() {
                $("html, body").animate({
                    scrollTop: $("#page" + $(this).attr("id")).offset().top + 'px'
                }, 'fast');
            });
        });
    </script>
    <style type="text/css">
        body {
	        background-color: #fff;
	        padding: 0;
	        margin: 0;
            margin-top: 30px;
        }

        img {
	        max-width: 100%;
	        border-bottom: 1px solid #3786ae;
        }

        img:last-child {
	        border-bottom: 0;
        }

        #pages {
            width: 100%;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        <div id="menu">
            <a id="PDF" href="Payout How-to.pdf" download>Download PDF</a>
            |&nbsp;&nbsp; Go to: 
            <a id="1" class="page">Tips</a>
            <a id="2" class="page">Import Data</a>
            <a id="4" class="page">Reports</a>
            <a id="6" class="page">Schedule</a>
            <a id="7" class="page">People</a>
            <a id="8" class="page">Carbon Copy</a>
            <a id="9" class="page">Executives</a>

            
        </div>

        <div id="pages">
            <img id="page1" src="Content/Howto1.png" />
            <br />
            <img id="page2" src="Content/Howto2.png" />
            <br />
            <img id="page3" src="Content/Howto3.png" />
            <br />
            <img id="page4" src="Content/Howto4.png" />
            <br />
            <img id="page5" src="Content/Howto5.png" />
            <br />
            <img id="page6" src="Content/Howto6.png" />
            <br />
            <img id="page7" src="Content/Howto7.png" />
            <br />
            <img id="page8" src="Content/Howto8.png" />
            <br />
            <img id="page9" src="Content/Howto9.png" />
        </div>

        <%----------------------------------------------------------------------------------------------------
        <table border="1" cellspacing="0" cellpadding="0" summary="Page layout for 2 interior booklet pages" > 
<tbody>
<tr>
<td valign="top" ><p>Table of Contents</p>

<p><b>Important Tips</b>........................................................................ 1</p>

<p><b>Import Data</b>............................................................................ 2</p>

<p><b>Reports</b>.................................................................................... 4 </p>

<p>Generating Reports............................................................... 4 </p>

<p>Export to Excel....................................................................... 4</p>

<p>Send via Email........................................................................ 5</p>

<p><b>Schedule</b>.................................................................................. 6</p>

<p>Search and Edit...................................................................... 6 </p>

<p>Add......................................................................................... 6 </p>

<p>Delete..................................................................................... 6 </p>

<p><b>People</b>...................................................................................... 7 </p>

<p>Missing Emails........................................................................ 7</p>

<p>Add, Edit and Delete............................................................. 7</p>

<p><b>Carbon Copy</b>.......................................................................... 8 </p>

<p><b>Executives</b>............................................................................... 9 </p>

<p>Add......................................................................................... 9</p>

<p>Edit and Delete...................................................................... 9</p></td>
<td valign="top" > </td>
<td valign="top" > </td>
<td valign="top" > <h1>Important Tips</h1>

<h2>Importing Data</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > </td>
<td valign="top" ><p>Always import the schedule first.</p>

<p>Schedule for different stores and programs can be imported at the same time. But sales data must be imported by store and program.</p>

<p>Supported Excel format: <b>XLSX</b></p></td>
</tr>

</tbody>
</table>

<h2>Sending Reports</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > </td>
<td valign="top" ><p>Always check for missing emails before sending reports via email.</p></td>
</tr>

</tbody>
</table>

</td>
</tr>

<tr>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" ><p>1</p></td>
</tr>

</tbody>
</table>

<table border="1" cellspacing="0" cellpadding="0" summary="Page layout for 2 interior booklet pages" > 
<tbody>
<tr>
<td valign="top" > <h1>Import Data</h1>

<p>1. Click the area in Figure 2.1 to select a file or drag and drop a file to the area.</p>

<p>2. Click “Upload” to send file to server and proceed to next step. Or click “Cancel” to select a different file.</p>

<p>3. Choose sheet name.</p>

<p><i>Make sure sheet name does not include spaces or special characters such as @, $, %, *, etc.</i></p>

<p>4. Select import type.</p>

<p><i>Note that when importing types other than “Schedule”, another dropdown appears for choosing the program.</i></p>

<p>5. Click “Save” in Figure 2.2 to import the data. Or click “Cancel” to upload and import a different file.</p></td>
<td valign="top" > </td>
<td valign="top" > </td>
<td valign="top" > <h1>Visuals</h1>

<h2>Figure 2.1</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

<h2>Figure 2.2</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

</td>
</tr>

<tr>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" ><p>2</p></td>
</tr>

</tbody>
</table>

<table border="1" cellspacing="0" cellpadding="0" summary="Page layout for 2 interior booklet pages" > 
<tbody>
<tr>
<td valign="top" > <h1>Import Data</h1>

<p>6. Once the import is complete, you will see a preview of what was imported.</p>

<p>See Figure 3.1</p>

<p>7. If the imported data is correct, click “Save Import” in Figure 3.1. Or click “Undo Import” to import a different file.</p>

<p><i>Note that once an import is saved, it cannot be deleted.</i></p></td>
<td valign="top" > </td>
<td valign="top" > </td>
<td valign="top" > <h1>Visuals</h1>

<h2>Figure 3.1</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

</td>
</tr>

<tr>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" ><p>3</p></td>
</tr>

</tbody>
</table>

<table border="1" cellspacing="0" cellpadding="0" summary="Page layout for 2 interior booklet pages" > 
<tbody>
<tr>
<td valign="top" > <h1>Reports</h1>

<h2>Generating Reports</h2>

<p>1. Select store</p>

<p>2. Select program</p>

<p><i>Note that stores and programs are pulled from the schedule. If a store or program is not on the dropdown list, then it is not on the schedule.</i></p>

<p>3. Select start date of rotation</p>

<p>4. Define length of rotation by number of days</p>

<p>5. Click “Generate” in Figure 4.1</p>

<p><i>Note that you can generate another report by clicking “New Report” in Figure 4.2.</i></p>

<h2>Export to Excel</h2>

<p>You can export summary, mix and all locations in one Excel file by clicking “Export All to Excel”. Or export each tab individually by clicking “Export this Location to Excel”.</p>

<p>See Figure 4.2.</p></td>
<td valign="top" > </td>
<td valign="top" > </td>
<td valign="top" > <h1>Visuals</h1>

<h2>Figure 4.1</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

<h2>Figure 4.2</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

</td>
</tr>

<tr>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" ><p>4</p></td>
</tr>

</tbody>
</table>

<table border="1" cellspacing="0" cellpadding="0" summary="Page layout for 2 interior booklet pages" > 
<tbody>
<tr>
<td valign="top" > <h1>Reports</h1>

<h2>Send via Email</h2>

<p><i>Always go to “People” before sending emails and make sure everyone on the schedule has an email address.</i></p>

<p><i>Go to page 7 for more details.</i></p>

<p>1. Click “Send via Email” in Figure 5.1.</p>

<p>2. Make sure you fill out “Executive Details”</p>

<p>3. Click “Send”</p>

</td>
<td valign="top" > </td>
<td valign="top" > </td>
<td valign="top" > <h1>Visuals</h1>

<h2>Figure 5.1</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

</td>
</tr>

<tr>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" ><p>5</p></td>
</tr>

</tbody>
</table>

<table border="1" cellspacing="0" cellpadding="0" summary="Page layout for 2 interior booklet pages" > 
<tbody>
<tr>
<td valign="top" > <h1>Schedule</h1>

<h2>Search and Edit</h2>

<p>You can find a record in the schedule by any of the column.</p>

<p>To edit a record, simply click the cell and start typing.</p>

<p><i>Note that changes are save as soon as a cell loses focus.</i></p>

<h2>Add</h2>

<p>To add a record to the schedule, click “Add” from the top menu (circled in Figure 6.1) and fill out all the fields.</p>

<p>Then click “Add” in Figure 6.2</p>

<h2>Delete</h2>

<p>To delete a record click on the red “X” at the beginning of the row.</p>

<p><i>Note that this action cannot be undone.</i></p></td>
<td valign="top" > </td>
<td valign="top" > </td>
<td valign="top" > <h1>Visuals</h1>

<h2>Figure 6.1</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

<h2>Figure 6.2</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

</td>
</tr>

<tr>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" ><p>6</p></td>
</tr>

</tbody>
</table>

<table border="1" cellspacing="0" cellpadding="0" summary="Page layout for 2 interior booklet pages" > 
<tbody>
<tr>
<td valign="top" > <h1>People</h1>

<h2>Missing Emails</h2>

<p>When you go to “People”, the app will notify you if there are any owners on the schedule that don’t have an email address (Figure 7.1).</p>

<p>These people are highlighted in red as seen in Figure 7.2.</p>

<p><i>Always check “People” before sending emails.</i></p>

<h2>Add, Edit and Delete</h2>

<p>To add a new person, click “Add Email” in Figure 7.2, fill in their information and click “Add”.</p>

<p>To Edit someone, simply click the field you want to update and type. Changes are saved as soon as a field loses focus.</p>

<p>To delete someone, click the red “X” next to their name.</p></td>
<td valign="top" > </td>
<td valign="top" > </td>
<td valign="top" > <h1>Visuals</h1>

<h2>Figure 7.1</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

<h2>Figure 7.2</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

</td>
</tr>

<tr>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" ><p>7</p></td>
</tr>

</tbody>
</table>

<table border="1" cellspacing="0" cellpadding="0" summary="Page layout for 2 interior booklet pages" > 
<tbody>
<tr>
<td valign="top" > <h1>Carbon Copy</h1>

<p>“Carbon Copy” is the email distribution list.</p>

<p>You can manage each person’s CCs for sending reports via email.</p>

<p>Anybody added to “People” will automatically show up in “Carbon Copy”.</p>

<p>Once someone is deleted from “People”, they will also be removed from “Carbon Copy” and anybody’s CCs.</p>

<p>To update CCs, simply click on the dropdown and select.</p>

<p><i>Note that CC dropdowns are also pulled from “People”.</i></p></td>
<td valign="top" > </td>
<td valign="top" > </td>
<td valign="top" > <h1>Visuals</h1>

<h2>Figure 8.1</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

</td>
</tr>

<tr>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" ><p>8</p></td>
</tr>

</tbody>
</table>

<table border="1" cellspacing="0" cellpadding="0" summary="Page layout for 2 interior booklet pages" > 
<tbody>
<tr>
<td valign="top" > <h1>Executives</h1>

<h2>Add</h2>

<p>1. Click “Add Executive” in Figure 9.1</p>

<p>2. Select the person from the first dropdown</p>

<p><i>Note that this dropdown is pulled from “People”.</i></p>

<p>3. Select the program they are the executive of </p>

<p><i>Note that this dropdown is pulled from “Schedule”.</i></p>

<p>4. Click “Add”</p>

<p><i>Note that you have to add the executive for each program that applies. If person A is an executive of programs B and C, you have to add person A twice, once for program B and once for program C.</i></p>

<h2>Edit and Delete</h2>

<p>To edit what program an executive who is already on the list should be assigned to, simply pick the program from the dropdown in the list.</p>

<p>To remove someone from the executive list, click on the red “X” next to their name.</p></td>
<td valign="top" > </td>
<td valign="top" > </td>
<td valign="top" > <h1>Visuals</h1>

<h2>Figure 9.1</h2>

<table border="1" cellspacing="0" cellpadding="0" summary="Photo with item description" > 
<tbody>
<tr>
<td valign="top" > <table border="1" cellspacing="0" cellpadding="0" summary="Picture frame" > 
<tbody>
<tr>
<td> </td>
</tr>

</tbody>
</table>

</td>
</tr>

</tbody>
</table>

</td>
</tr>

<tr>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" > </td>
<td valign="bottom" ><p>9</p></td>
</tr>

</tbody>
</table>

        ----------------------------------------------------------------------------------------------------%>

    </div>
    </form>

</body>
</html>
