-- phpMyAdmin SQL Dump
-- version 3.5.1
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: May 03, 2013 at 07:16 PM
-- Server version: 5.5.24-log
-- PHP Version: 5.4.3

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `shids`
CREATE DATABASE `shids` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `shids`;

CREATE TABLE IF NOT EXISTS `black_board` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `i_id` bigint(20) NOT NULL,
  `action_type` int(11) NOT NULL,
  `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `status` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_bb_i_id_idx` (`i_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=9 ;

--
-- Dumping data for table `black_board`
--


-- --------------------------------------------------------

--
-- Table structure for table `black_board_data`
--

CREATE TABLE IF NOT EXISTS `black_board_data` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `b_id` bigint(20) NOT NULL,
  `date` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data` longtext NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_bbd_bid_idx` (`b_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- Dumping data for table `black_board_data`
--


-- --------------------------------------------------------

--
-- Table structure for table `incidents`
--

CREATE TABLE IF NOT EXISTS `incidents` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `timeStamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `sourceIp` varchar(20) COLLATE utf8_bin NOT NULL,
  `type` int(9) NOT NULL,
  `desc` varchar(200) COLLATE utf8_bin NOT NULL,
  `sha1` varchar(180) COLLATE utf8_bin DEFAULT NULL,
  `status` int(2) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=19 ;

--
-- Dumping data for table `incidents`
--

--
-- Constraints for dumped tables
--

--
-- Constraints for table `black_board`
--
ALTER TABLE `black_board`
  ADD CONSTRAINT `fk_bb_i_id` FOREIGN KEY (`i_id`) REFERENCES `incidents` (`id`) ON UPDATE NO ACTION;

--
-- Constraints for table `black_board_data`
--
ALTER TABLE `black_board_data`
  ADD CONSTRAINT `fk_bbd_bid` FOREIGN KEY (`b_id`) REFERENCES `black_board` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `incidents` ADD `syslogd_status` INT NOT NULL DEFAULT '0' AFTER `status`;
ALTER TABLE  `incidents` CHANGE  `timeStamp`  `timeStamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE  `incidents` CHANGE  `timeStamp`  `timeStamp` TIMESTAMP NOT NULL;
ALTER TABLE  `incidents` CHANGE  `timeStamp`  `timeStamp` TIMESTAMP ON UPDATE CURRENT_TIMESTAMP NOT NULL DEFAULT  '0000-00-00 00:00:00.000'
