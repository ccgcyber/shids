<?php require_once("Db.Class.php"); ?>
<?php require_once("Config.inc.php"); ?>
<?php
$viewState = array();
$viewState['srcIp'] = DEFAULT_FIRST_VALUE;
unset ($records);
if (isset($_POST)) {
    if (isset($_POST['srcIp'])) {
        if ($_POST['srcIp'] != DEFAULT_FIRST_VALUE) {
            $viewState['srcIp'] =trim($_POST['srcIp']);
            $records =Db::getBBDataByIP(trim($_POST['srcIp']));
        }
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
            function popupVt(url) {
                popupWindow = window.open(url, 'popUpWindow', 'height=500,width=800,left=100,top=100,resizable=yes,scrollbars=yes,toolbar=yes,menubar=no,location=no,directories=no, status=yes');
            }
        </script>
    </head>
    <body>
        <form name="fFilter" action="" method="post">
            <h1>Black Board</h1>
            <table class="tableRecords">
                <thead>
                    <tr>
                        <td>
                            IP
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
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
                            <input type="submit" name="Search" value="Search"/>
                        </td>
                    </tr>
                </thead>
                <tbody>
                    <?php ?>
                </tbody>
                <tfoot>
                    <tr>
                        <td colspan="2">
                        </td>
                    </tr>
                </tfoot>
            </table>
        </form>
        <?php
        if (isset($records) ) :
            if (is_array($records) ) :
                if ( count($records)>0) :?>

        <table class="tableRecords">
            <caption>Black Board Details</caption>
            <thead>
                <tr>
                    <th>Date</th>
                    <th>IP</th>
                    <th>Incident</th>
                    <th>Action</th>
                    <th>Status</th>
                    <th>View Data</th>
                </tr>
            </thead>
            <tbody>

                            <?php foreach ($records as $key => $value) :?>
                <tr class="<?php echo getBBStatusText($value['status'],BB_STATUS_RETURN_TYPE_CSS);?>">
                    <td><?php print($value['date']);?></td>
                    <td><?php print($value['sourceIp']);?></td>
                    <td><?php print($value['desc']);?></td>
                    <td><?php print(getActionText($value['action_type']));?></td>
                    <td><?php print(getBBStatusText($value['status']));?></td>
                    <td><a href="viewBBAgentData.php?bid=<?php print($value['id']);?>" onclick="popupVt(this.href);return false">View Data</a></td>
                </tr>
                            <?php endforeach;?>
            </tbody>

        </table>
                <?php
                endif;
            endif;
        endif;
        ?>
    </body>
</html>