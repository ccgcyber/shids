<?php
define("TITLE","Simple - Host Incidence Detection System");
define("VERSION","{ Alpha 0.1 }");
define("DEVELOPED_BY","By:- Nisansala, Sithum, Rajitha, Ruwan, Gayan");

define('HOST', 'localhost');
define('USERNAME', 'root1');
define('PASSWORD', '');
define('DATABASE', 'shids');

define('TYPE_FILE','1');
define('TYPE_REG','2');
define('TYPE_FILE_TEXT','File Modification');
define('TYPE_REG_TEXT','Registry Modification');

define('STATUS_OK','0');
define('STATUS_ERROR','1');

define('RECENT_MAX','20');

define('DEFAULT_FIRST_VALUE','-1');
define('DEFAULT_FIRST_VALUE_TEXT','Select a value');

define('DEFAULT_TIME_OUT_PERIOD','60');

define('DEFAULT_JS_DATE_FORMAT','yy-mm-dd');

define('VIRUS_TOTAL_API','d2252c8010b206aef46095fe19d4e994771963f5194a3ad0a1a397a360c29892');

define('ACTION_TRACK_PROCESS','1');
define('ACTION_TRACK_PROCESS_TEXT','Track process');

define('BB_STATUS_NEW','1');
define('BB_STATUS_AGENT_RECEIVED','2');
define('BB_STATUS_AGENT_SENT_RESULT','3');
define('BB_STATUS_AGENT_ERROR','4');

define('BB_STATUS_NEW_TEXT','Newly Added Entry');
define('BB_STATUS_AGENT_RECEIVED_TEXT','Agent Still Processing');
define('BB_STATUS_AGENT_SENT_RESULT_TEXT','Agent Sent Result');
define('BB_STATUS_AGENT_ERROR_TEXT','Agent Reported Error');

define("BB_STATUS_RETURN_TYPE_TEXT",1);
define("BB_STATUS_RETURN_TYPE_CSS",2);

function getTypeText($type) {

    switch ($type) {
        case TYPE_FILE:
            return TYPE_FILE_TEXT;
        case TYPE_REG:
            return TYPE_REG_TEXT;
    }
}
function getTypes() {
    return array(TYPE_FILE=>TYPE_FILE_TEXT,TYPE_REG=>TYPE_REG_TEXT);
}

function getBBStatusText($status,$type=1) {
    switch ($status) {
        case BB_STATUS_NEW:
            return $type==BB_STATUS_RETURN_TYPE_TEXT?BB_STATUS_NEW_TEXT:"status_new";

        case BB_STATUS_AGENT_RECEIVED:
            return $type==BB_STATUS_RETURN_TYPE_TEXT?BB_STATUS_AGENT_RECEIVED_TEXT:"status_process";

        case BB_STATUS_AGENT_SENT_RESULT:
            return $type==BB_STATUS_RETURN_TYPE_TEXT?BB_STATUS_AGENT_SENT_RESULT_TEXT:"status_sentresult";

        case BB_STATUS_AGENT_ERROR:
            return $type==BB_STATUS_RETURN_TYPE_TEXT?BB_STATUS_AGENT_ERROR_TEXT:"status_error";
    }
}
function getActionText($action) {
    switch ($action) {
        case ACTION_TRACK_PROCESS:
            return ACTION_TRACK_PROCESS_TEXT;
    }
}



?>
