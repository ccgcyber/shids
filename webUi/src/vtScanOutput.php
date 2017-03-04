<?php require_once("Config.inc.php"); ?>
<?php include('VirusTotalAPIV2.php'); ?>
<html>
    <head>
        <?php include_once 'header.inc.php'; ?>
        <meta http-equiv="Refresh" content="<?php echo DEFAULT_TIME_OUT_PERIOD; ?>">

    </head>

    <body>
        <?php
        echo "<h2>Virus Total Scan history <h2><br/><h3>File :" . base64_decode(trim($_GET['fn'])) . '</h3></h2>';


        $api = new VirusTotalAPIV2(VIRUS_TOTAL_API);
        $report = $api->getFileReport(strtolower(base64_decode(trim($_GET['sha1']))));
        if ($report) {
            echo "<br/>";
            print("<h3>Scanned Date: " . $api->getSubmissionDate($report) . "</h3>");
            echo "<br/>";

            print('<h3><a href="' . $api->getReportPermalink($report, TRUE) . '" target="_blank">Click here to view the scan details</a></h3>');
        } else {
            echo "<h3>No data found.</h3>";
        }
        ?>
    </body>
</html>