<?php require_once('Db.Class.php'); ?>
<?php include_once 'Config.inc.php'; ?>

<?php
if (isset($_GET['incidentId'])) {

    $records=Db::checkBBDataByIncidentId(trim($_GET['incidentId']));

    if (is_array($records) ) {
        if ( count($records)==0) {
            $dataArray['i_id']=trim($_GET['incidentId']);
            $dataArray['action_type']=ACTION_TRACK_PROCESS;
            $dataArray['status']=BB_STATUS_NEW;
            Db::insertBlackBoardData($dataArray);
            
        }
    }
    $records=Db::getBBDataByIncidentId(trim($_GET['incidentId']));
}
?>
<html>
    <head>
        <?php include_once 'header.inc.php'; ?>
        <script>
            function popupVt(url) {
                popupWindow = window.open(url, 'popUpWindow', 'height=500,width=800,left=100,top=100,resizable=yes,scrollbars=yes,toolbar=yes,menubar=no,location=no,directories=no, status=yes');
            }
        </script>
    </head>
    <body>
        <table class="tableRecords">
            <caption>Black Board Details</caption>
            <thead>
                <tr>
                    <th>IP</th>
                    <th>Incident</th>                    
                    <th>Action</th>
                    <th>Status</th>
                    <th>Date</th>
                    <th>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</th>
                </tr>
            </thead>
            <tbody>
                <?php foreach ($records as $key => $value): ?>
                <tr class="<?php echo getBBStatusText($value['status'],BB_STATUS_RETURN_TYPE_CSS);?>">
                    <td><?php print($value['sourceIp']);?></td>
                    <td><?php print($value['desc']);?></td>
                    <td><?php print(getActionText($value['action_type']));?></td>
                    <td><?php print(getBBStatusText($value['status']));?></td>
                    <td><?php print($value['date']);?></td>
                    <td><a href="viewBBAgentData.php?bid=<?php print($value['id']);?>" onclick="popupVt(this.href);return false">View Data</a></td>
                </tr>
                <?php endforeach; ?>
            </tbody>

        </table>
    </body>
</html>