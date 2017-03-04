<?php require_once('Db.Class.php'); ?>
<?php include_once 'Config.inc.php'; ?>
<?php
if (trim($_GET['id'])&& trim($_GET['status'])) {
    $records=Db::updateBlackBoard(array("id"=>trim($_GET['id']),"status"=>trim($_GET['status'])));// trim($_GET['id']), BB_STATUS_AGENT_RECEIVED);
}
?>