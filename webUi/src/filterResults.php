<?php require_once('Db.Class.php'); ?>
<?php include_once 'Config.inc.php'; ?>
<?php
unset($records);
$viewState = array();
$viewState['srcIp'] = DEFAULT_FIRST_VALUE;
$viewState['type'] = DEFAULT_FIRST_VALUE;
unset($viewState['fromDate']);

if (isset($_POST)) {
    if (isset($_POST['Search'])) {
        $viewState['fromDate'] = $_POST['fromDate'];
        $filter = null;

        $viewState['srcIp'] = trim($_POST['srcIp']);
        $viewState['type'] = trim($_POST['type']);

        if (isset($_POST['srcIp'])) {
            if ($_POST['srcIp'] != DEFAULT_FIRST_VALUE) {
                if (!is_array($filter)) {
                    $filter = array();
                }
                $filter['sourceIp'] = trim($_POST['srcIp']);
            }
        }
        if (isset($_POST['type'])) {
            if ($_POST['type'] != DEFAULT_FIRST_VALUE) {
                if (!is_array($filter)) {
                    $filter = array();
                }
                $filter['type'] = trim($_POST['type']);
            }
        }
        if (isset($_POST['fromDate'])) {
            if (trim($_POST['fromDate']) != "") {
                if (!is_array($filter)) {
                    $filter = array();
                }
                $filter['fromDate'] = trim($_POST['fromDate']);
            }
        }
        $records = Db::getIncidentsFiltered($filter);
    }
}
?>

<html>
    <head>
        <?php include_once 'header.inc.php'; ?>
        <script>
            $(function() {
                $("input[type=submit]")
                .button();
            });
            $(function() {
                $("#fromDate").datepicker({
                    //defaultDate: "-1w",
                    changeMonth: true,
                    changeYear: true,
                    //numberOfMonths: 2,
                    dateFormat: "<?php echo DEFAULT_JS_DATE_FORMAT ?>",
                    onSelect: function(selectedDate) {
                        $("#fromDate").datepicker("option", "minDate", selectedDate);
                    }
                });
            });

            function popupVt(url) {
                popupWindow = window.open(url, 'popUpWindow', 'height=500,width=500,left=100,top=100,resizable=yes,scrollbars=yes,toolbar=yes,menubar=no,location=no,directories=no, status=yes');
            }
        </script>

    </head>
    <body>
        <form name="fFilter" action="" method="post">
            <h1>Filtered Results </h1>
            <table class="tableRecords">
                <thead>
                    <tr>
                        <td>
                            Date
                        </td>
                        <td>
                            IP
                        </td> 
                        <td>
                            Type
                        </td>  
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input type="text" name="fromDate" id="fromDate"  <?php echo isset($viewState['fromDate']) ? "value=\"" . $viewState['fromDate'] . "\"" : false ?>/>
                        </td>
                        <td>
                            <select name="srcIp">
                                <option value="<?php echo DEFAULT_FIRST_VALUE; ?>"><?php echo DEFAULT_FIRST_VALUE_TEXT; ?></option>
                                <?php
                                $results = Db::getSrcIPs();
                                if (is_array($results)):
                                    foreach ($results as $result => $value):
                                        ?>
                                <option <?php echo $viewState['srcIp'] == $value['sourceIp'] ? 'Selected="true"' : ""; ?> value="<?php echo $value['sourceIp'] ?>"><?php echo $value['sourceIp'] ?></option>
                                    <?php
                                    endforeach;
                                endif;
                                ?>
                            </select>
                        </td>
                        <td>
                            <select name="type">
                                <option value="<?php echo DEFAULT_FIRST_VALUE; ?>"><?php echo DEFAULT_FIRST_VALUE_TEXT; ?></option>
                                <?php
                                $results = getTypes();
                                if (is_array($results)):
                                    foreach ($results as $result => $value):
                                        ?>
                                <option <?php echo $viewState['type'] == $result ? 'Selected="true"' : ""; ?> value="<?php echo $result ?>"><?php echo $value ?></option>
                                    <?php
                                    endforeach;
                                endif;
                                ?>
                            </select>
                        </td>    
                        <td>
                            <input type="submit" name="Search" value="Search"/>
                        </td>
                    </tr>
                </thead>
                <tbody>
                    <?php ?>
                </tbody>
                <tfoot>
                    <tr>
                        <td colspan="4">
                        </td>
                    </tr>
                </tfoot>
            </table>
        </form>
        <?php if (isset($records)): ?>
        <table class="tableRecords">
            <thead>
                <tr>
                    <td>
                        Date Time
                    </td>
                    <td>
                        IP
                    </td>
                    <td>
                        Type
                    </td>
                    <td>
                        Description
                    </td>
                </tr>
            </thead>
            <tbody>
                    <?php
                    $count = 0;
                    if (is_array($records)):
                        foreach ($records as $record => $value):
                            ?>
                <tr <?php echo $value['type'] == TYPE_FILE ? 'class="fileModification"' : 'class="regModification"'; ?> >
                    <td><?php echo $value['timeStamp']; ?></td>
                    <td><?php echo $value['sourceIp']; ?></td>
                    <td><?php echo getTypeText($value['type']); ?></td>
                    <td>
                                    <?php echo nl2br($value['desc']); ?>
                                    <?php if ($value['type']==TYPE_FILE && $value['sha1']!=false ): ?>
                        <ul>
                            <!-- <li><a href="vtScanOutput.php?sha1=<?php echo base64_encode($value['sha1'])?>&fn=<?php echo base64_encode($value['desc'])?>" onclick="popupVt(this.href);return false">SHA1: <?php echo strtolower($value['sha1']); ?> &nbsp;[<-VirusTotal.com]</a></li> -->
                            <li><a href="trackProcess.php?incidentId=<?php echo $value['id']?>" onclick="popupVt(this.href);return false">Track Process</a></li>
                        </ul>
                                    <?php endif;?>
                    </td>
                </tr>
                            <?php
                            $count++;
                        endforeach;
                        ?>
                    <?php else : ?>
                <tr>
                    <td colspan="4">No Incidents found</td>
                </tr>
                    <?php endif; ?>
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="4">
                        No. of records found: <?php echo $count; ?>
                    </td>
                </tr>
            </tfoot>
        </table>
        <?php endif; ?>
    </body>
</html>
