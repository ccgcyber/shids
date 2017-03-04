<?php require_once("Db.Class.php"); ?>
<?php require_once("Config.inc.php"); ?>
<?php

if (trim($_GET['bid'])) {
    $records=Db::getBBAgentDataByBBId(trim($_GET['bid']));
}
?>

<html>
    <head>
        <?php include_once 'header.inc.php'; ?>
        <script language="javascript">
            function toggleDiv(divid){
                if(document.getElementById(divid).style.display == 'none'){
                    document.getElementById(divid).style.display = 'block';
                }
                else{
                    document.getElementById(divid).style.display = 'none';}
            }
        </script>
    </head>
    <body>
        <table  class="tableRecords">
            <?php
            if (isset($records) ) :
                if (is_array($records) ) :
                    if ( count($records)>0) :?>
            <tr>
                <td>
                    Date
                </td>
                <td><?php print($records[0]['bbdDate'])?></td>
            </tr>
            <tr>
                <td colspan="2">Data</td>
            </tr>
            <tr>
                <td colspan="2">
                    <ul><?php
                                    $s=str_replace("[Begin Process Info]", "<li><a href=\"javascript:;\" onmousedown=\"toggleDiv('pinfo');\">Process Info</a><br/><br/><div style=\"display:none;\" id=\"pinfo\">", nl2br($records[0]['data']));
                                    $s=str_replace("[End Process Info]", "</div></li><br/>", $s);

                                    $s=str_replace("[Begin Netstat info]", "<li><a href=\"javascript:;\" onmousedown=\"toggleDiv('ninfo');\">NetStat Info</a><br/><br/><div style=\"display:none;\" id=\"ninfo\">", $s);
                                    $s=str_replace("[End Netstat Info]", "</div></li><br/>", $s);

                                    $s=str_replace("[Begin File info]", "<li><a href=\"javascript:;\" onmousedown=\"toggleDiv('finfo');\">File Info</a><br/><br/><div style=\"display:none;\" id=\"finfo\">", $s);

                                    $data=explode("[Begin File Data]", $s);
                                    $exe=null;
                                    $s=str_replace("[End File info]", "</div></li>", $s);
                                    
                                    if (count($data)>1) {

                                        $data=explode("[End File Data]", $data[1]);
                                        if (count($data)>1) {
                                            $s=str_replace($data[0], '', $s);
                                            $s=str_replace("[Begin File Data]","<br/><textarea name=\"fileData\" cols=\"25\" rows=\"5\" style=\"width:100%;height:300px;\">".base64_decode($data[0]), $s);
                                            $s=str_replace("[End File Data]","</textarea><br/>", $s);
                                            
                                        }
                                    }
                                    print($s);?>
                    </ul>
                </td>
            </tr>
                    <?php
                    endif;
                endif;
            endif;
            ?>
        </table>
    </body>
</html>
