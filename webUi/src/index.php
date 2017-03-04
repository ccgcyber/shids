<?php require_once("Config.inc.php"); ?>
<html>
    <head>
        <?php include_once 'header.inc.php'; ?>

    </head>
    <body>
        <h1 class="ui-state-default ui-corner-top"><?php echo TITLE . "&nbsp;&nbsp;" . VERSION; ?></h1>
        <table>
            <thead>
                <tr>
                    <td>
                        <div id="tabs"  id="tabs" class="ui-tabs ui-widget ui-widget-content ui-corner-all">
                            <ul class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all" >
                                <li class="ui-state-default ui-corner-top ui-tabs-active ui-state-active ui-state-focus"><a href="incidentReport.php" target="iBody"  style="cursor: pointer;">View Reports</a></li>
                                <li class="ui-state-default ui-corner-top ui-tabs-active ui-state-active ui-state-focus"><a href="filterResults.php" target="iBody"  style="cursor: pointer;">Filter</a></li>
                                <li class="ui-state-default ui-corner-top ui-tabs-active ui-state-active ui-state-focus"><a href="viewBlackBoard.php" target="iBody"  style="cursor: pointer;">View Black Board</a></li>
                            </ul>
                        </div>
                    </td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <iframe name="iBody" src="welcome.php"/>
                    </td>
                </tr>
            </tbody>
        </table>
    </body>
</html>