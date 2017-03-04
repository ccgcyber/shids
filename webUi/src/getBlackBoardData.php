<?php require_once('Db.Class.php'); ?>
<?php include_once 'Config.inc.php'; ?>
<?php
if (trim($_GET['sip'])) {
    $records=Db::getBBDataByIPandStatus(trim($_GET['sip']), BB_STATUS_NEW);    
    if (is_array($records) ) {
        if ( count($records)>0) {
            foreach ($records as $key => $value) {
                echo $value['id']."|".$value['sha1']."<br/>";
            }
        }
    }
}

?>
