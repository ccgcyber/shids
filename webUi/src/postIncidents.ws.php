<?php
//require_once("Config.inc.php");
require_once("Db.Class.php");
//require_once("lib/nusoap.php");
    function ReportIncident($timeStamp, $sIp, $type, $desc,$sha1) {
        return Db::insertData(array("timeStamp" => $timeStamp, "sourceIp" => $sIp, "type" => $type, "desc" => $desc, "sha1" => $sha1,"status" => STATUS_OK));
    }

if(isset($_POST)) {
    if(isset($_POST['post'])) {
        if (trim($_POST['timeStamp'])!=false &&
                trim($_POST['sourceIp'])!=false &&
                trim($_POST['type'])!=false&&
                trim($_POST['desc'])!=false) {
            
            $sha1=trim($_POST['sha1']);
            ReportIncident(trim($_POST['timeStamp']),
                    trim($_POST['sourceIp']),
                    trim($_POST['type']),
                    trim($_POST['desc']),
                    $sha1);
        }
    }
}
?>
