<?php
require_once("Db.Class.php");
//require_once("lib/nusoap.php");
function addBBData($bId,$data) {
    Db::updateBlackBoard(array("id" => $bId, "status" => BB_STATUS_AGENT_SENT_RESULT));
    return Db::insertBlackBoardAgentData(array("b_id" => $bId, "data" => $data));
}

if(isset($_POST)) {
    if(isset($_POST['post'])) {
        if (trim($_POST['b_id'])!=false &&
                trim($_POST['data'])!=false) {
            
            addBBData(trim($_POST['b_id']),
                    trim($_POST['data']));
        }
    }
}

?>
