<?php require_once("Db.Class.php"); ?>
<?php require_once("Config.inc.php"); ?>
<html>
    <head>
        <?php include_once 'header.inc.php'; ?>
        <meta http-equiv="Refresh" content="<?php echo DEFAULT_TIME_OUT_PERIOD; ?>">
        <script>
            function popupVt(url) {
                popupWindow = window.open(url, 'popUpWindow', 'height=500,width=800,left=100,top=100,resizable=yes,scrollbars=yes,toolbar=yes,menubar=no,location=no,directories=no, status=yes');
            }
        </script>
        <?php
        $records = Db::getIncidents(RECENT_MAX);
        ?>
    </head>
    <body>
        <h1>Recent <?php echo RECENT_MAX ?> Incidences </h1>
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
    </body>
</html>