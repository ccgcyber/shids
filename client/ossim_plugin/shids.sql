-- SHIDS
-- plugin_id: 9851
DELETE FROM plugin WHERE id = "9851";
DELETE FROM plugin_sid where plugin_id = "9851";

INSERT INTO plugin (id,type,name,description) VALUES (9851,1,'S-HIDS','Simple Host Incident Detection System');
INSERT INTO plugin_sid (plugin_id,sid,category_id,class_id,name,priority,reliability) VALUES (9851,1,NULL,NULL,'File Modification',3,2);
INSERT INTO plugin_sid (plugin_id,sid,category_id,class_id,name,priority,reliability) VALUES (9851,2,NULL,NULL,'Registry Modification',3,2);
