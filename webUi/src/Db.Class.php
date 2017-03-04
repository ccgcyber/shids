<?php

require_once("Config.inc.php");

class Db {

    private static $con = null;

    private static function getConnection() {

        try {
            if (Db::$con == null) {
                Db::$con = new PDO("mysql:host=" . HOST . ";dbname=" . DATABASE, USERNAME, PASSWORD);
            }

            return Db::$con;
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }

    public static function insertData($dataArray) {

        try {
            $sql = "INSERT INTO incidents (`timeStamp`,`sourceIp`,`type`,`desc`,`sha1`,`status`) VALUES (:timeStamp,:sourceIp,:type,:desc,:sha1,:status)";
            $q = Db::getConnection()->prepare($sql);

            $q->bindParam(":timeStamp", $dataArray['timeStamp']);
            $q->bindParam(":sourceIp", $dataArray['sourceIp']);
            $q->bindParam(":type", $dataArray['type']);
            $q->bindParam(":desc", $dataArray['desc']);
            $q->bindParam(":sha1", $dataArray['sha1']);
            $q->bindParam(":status", $dataArray['status']);

            $q->execute();
            return true;
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }
    public static function insertBlackBoardData($dataArray) {

        try {
            $sql = "INSERT INTO black_board (`i_id`,`action_type`,`date`,`status`) VALUES (:i_id,:action_type,NOW(),:status)";
            $q = Db::getConnection()->prepare($sql);

            $q->bindParam(":i_id", $dataArray['i_id']);
            $q->bindParam(":action_type", $dataArray['action_type']);
            $q->bindParam(":status", $dataArray['status']);

            $q->execute();
            return true;
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }
    
    public static function insertBlackBoardAgentData($dataArray) {

        try {
            $sql = "INSERT INTO black_board_data  (`b_id`,`date`,`data`) VALUES (:a_b_id,NOW(),:a_data)";
            $q = Db::getConnection()->prepare($sql);

            $q->bindParam(":a_b_id", $dataArray['b_id']);
            $q->bindParam(":a_data", $dataArray['data']);

            $q->execute();
            return true;
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }

    public static function updateBlackBoard($dataArray){
        try {
            $sql = "UPDATE black_board  SET status=:u_status WHERE id=:iId";
            $q = Db::getConnection()->prepare($sql);

            $q->bindParam(":iId", $dataArray['id']);
            $q->bindParam(":u_status", $dataArray['status']);

            $q->execute();
            return true;
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }
    
    public static function getIncidents($limit = 0) {
        try {
            $sql = "SELECT * FROM incidents ORDER BY timeStamp DESC";
            if ($limit > 0) {
                $sql = $sql . " LIMIT $limit";
            }
            $q = Db::getConnection()->query($sql);
            $q->setFetchMode(PDO::FETCH_ASSOC);
            return $q->fetchAll();
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }
    public static function getBBDataByIncidentId($iId) {
        try {
            $sql = "SELECT incidents.id as inId,incidents.sourceIp,incidents.sourceIp,incidents.desc,incidents.sha1,black_board.*
                FROM black_board
                INNER JOIN incidents ON incidents.`id` = black_board.`i_id` WHERE black_board.i_id=:iid ORDER BY black_board.date DESC";

            $para = array();
            $para['iid'] = $iId;

            $q = Db::getConnection()->prepare($sql);
            $q->setFetchMode(PDO::FETCH_ASSOC);

            $q->execute($para);

            return $q->fetchAll();
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }
    public static function checkBBDataByIncidentId($iId) {
        try {
            $sql = "SELECT incidents.id as inId,incidents.sourceIp,incidents.sourceIp,incidents.desc,incidents.sha1,black_board.*
                FROM black_board
                INNER JOIN incidents ON incidents.`id` = black_board.`i_id` WHERE black_board.i_id=:iid AND black_board.status<:bStatus";

            $para = array();
            $para['iid'] = $iId;
            $para['bStatus'] = BB_STATUS_AGENT_SENT_RESULT;

            $q = Db::getConnection()->prepare($sql);
            $q->setFetchMode(PDO::FETCH_ASSOC);

            $q->execute($para);

            return $q->fetchAll();
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }
    public static function getBBDataByIncidentIdandStatus($iId,$status) {
        try {
            $sql = "SELECT incidents.id as inId,incidents.sourceIp,incidents.sourceIp,incidents.desc,incidents.sha1,black_board.*
                FROM black_board
                INNER JOIN incidents ON incidents.`id` = black_board.`i_id` WHERE black_board.i_id=:iid AND black_board.status=:i_status";
            $para = array();
            $para['iid'] = $iId;
            $para['i_status'] = $status;


            $q = Db::getConnection()->prepare($sql);
            $q->setFetchMode(PDO::FETCH_ASSOC);

            $q->execute($para);

            return $q->fetchAll();
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }
    public static function getBBDataByIPandStatus($sip,$status) {
        try {
            $sql = "SELECT incidents.id as inId,incidents.sourceIp,incidents.sourceIp,incidents.desc,incidents.sha1,black_board.*
                FROM black_board
                INNER JOIN incidents ON incidents.`id` = black_board.`i_id` WHERE incidents.sourceIp=:sip AND black_board.status=:i_status";
            $para = array();
            $para['sip'] = $sip;
            $para['i_status'] = $status;


            $q = Db::getConnection()->prepare($sql);
            $q->setFetchMode(PDO::FETCH_ASSOC);

            $q->execute($para);

            return $q->fetchAll();
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }
    public static function getBBDataByIP($sip) {
        try {
            $sql = "SELECT incidents.id as inId,incidents.sourceIp,incidents.sourceIp,incidents.desc,incidents.sha1,black_board.*
                FROM black_board
                INNER JOIN incidents ON incidents.`id` = black_board.`i_id` WHERE incidents.sourceIp=:sip ORDER by black_board.date DESC";
            $para = array();
            $para['sip'] = $sip;

            $q = Db::getConnection()->prepare($sql);
            $q->setFetchMode(PDO::FETCH_ASSOC);

            $q->execute($para);

            return $q->fetchAll();
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }

    public static function getBBAgentDataByBBId($bid) {
            try {
            $sql = "SELECT black_board_data.id as bbdId,black_board_data.b_id,black_board_data.date as bbdDate,black_board_data.data, black_board.*
                FROM black_board_data                
                INNER JOIN black_board ON black_board.`id` = black_board_data.b_id
                WHERE black_board_data.b_id=:bid";
            $para = array();
            $para['bid'] = $bid;

            $q = Db::getConnection()->prepare($sql);
            $q->setFetchMode(PDO::FETCH_ASSOC);

            $q->execute($para);

            return $q->fetchAll();
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }

    public static function getSrcIPs() {
        try {
            $sql = "SELECT sourceIp FROM incidents GROUP BY sourceIp";
            $q = Db::getConnection()->query($sql);
            //$q->setFetchMode(PDO::FETCH_ASSOC);
            return $q->fetchAll();
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }

    public static function getIncidentsFiltered($filters, $limit = 0) {
        try {
            $sql = "SELECT * FROM incidents";
            $and = false;
            $para = null;
            if (isset($filters['sourceIp'])) {
                $para = array();
                $para['sip'] = trim($filters['sourceIp']);
                $and = true;
                $sql = $sql . " WHERE sourceIp=:sip";
            }
            if (isset($filters['type'])) {
                if ($and == true) {
                    $sql = $sql . " AND ";
                } else {
                    $sql = $sql . " WHERE ";
                }
                if (is_array($para)) {
                    $para['t'] = trim($filters['type']);
                } else {
                    $para = array();
                    $para['t'] = trim($filters['type']);
                }
                $sql = $sql . " type=:t";
            }
            if (isset($filters['fromDate'])) {
                if ($and == true) {
                    $sql = $sql . " AND ";
                } else {
                    $sql = $sql . " WHERE ";
                }
                if (is_array($para)) {
                    $para['ts1'] = trim($filters['fromDate'])." 00:00:00";

                } else {
                    $para = array();
                    $para['ts1'] = trim($filters['fromDate'])." 00:00:00";
                }
                $para['ts2'] = date('Y-m-d', strtotime("+1 day",strtotime(trim($filters['fromDate']))))." 00:00:00";

                $sql = $sql . " timeStamp BETWEEN :ts1 AND :ts2 ";
                $sql = $sql ." ";
            }
            $sql = $sql . " ORDER BY timeStamp DESC";
            if ($limit > 0) {
                $sql = $sql . " LIMIT $limit";
            }

            $q = Db::getConnection()->prepare($sql);

            if (is_array($para)) {
                $q->execute($para);
            } else {
                $q->execute();
            }
            return $q->fetchAll();
        } catch (Exception $e) {
            throw new Exception($e->getMessage());
        }
    }

}

?>
