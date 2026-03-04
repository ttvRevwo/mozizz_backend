-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2026. Már 04. 09:16
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `mozizz`
--
CREATE DATABASE IF NOT EXISTS `mozizz` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `mozizz`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `emaillogs`
--

CREATE TABLE `emaillogs` (
  `email_log_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `email_type` varchar(50) DEFAULT NULL,
  `subject` varchar(255) DEFAULT NULL,
  `body` text DEFAULT NULL,
  `sent_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `status` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `emaillogs`
--

INSERT INTO `emaillogs` (`email_log_id`, `user_id`, `email_type`, `subject`, `body`, `sent_at`, `status`) VALUES
(1, 2, 'Reservation', 'Foglalás visszaigazolás', 'A foglalásod sikeresen rögzítve lett.', '2025-12-03 17:50:52', 'Sent');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `halls`
--

CREATE TABLE `halls` (
  `hall_id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `location` varchar(255) DEFAULT NULL,
  `seating_capacity` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `halls`
--

INSERT INTO `halls` (`hall_id`, `name`, `location`, `seating_capacity`) VALUES
(1, 'Főterem', '1. emelet', 100),
(2, 'Kisterem', '2. emelet', 50);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `movies`
--

CREATE TABLE `movies` (
  `movie_id` int(11) NOT NULL,
  `title` varchar(255) NOT NULL,
  `genre` varchar(100) DEFAULT NULL,
  `duration` int(11) NOT NULL,
  `rating` varchar(10) DEFAULT NULL,
  `description` text DEFAULT NULL,
  `img` varchar(255) DEFAULT NULL,
  `release_date` date DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `movies`
--

INSERT INTO `movies` (`movie_id`, `title`, `genre`, `duration`, `rating`, `description`, `img`, `release_date`, `created_at`) VALUES
(1, 'Avatar', 'Sci-Fi', 162, 'PG-13', 'Sci-Fi kalandfilm', 'movies/suti.jpg', '2009-12-18', '2025-12-03 17:50:52'),
(2, 'Joker', 'Drama', 122, 'R', 'Thriller/dráma', 'movies/joghurtos-barackos-suti-2-yxMq7y.webp', '2019-10-04', '2025-12-03 17:50:52'),
(3, 'dwadwa', 'dwadawda', 222, '2dadas', 'dadada', 'movies/szar.png', '2026-02-26', '2026-02-26 08:34:33');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `payments`
--

CREATE TABLE `payments` (
  `payment_id` int(11) NOT NULL,
  `reservation_id` int(11) NOT NULL,
  `payment_method` varchar(50) NOT NULL,
  `amount` decimal(10,2) NOT NULL,
  `payment_status` varchar(50) NOT NULL,
  `payment_date` timestamp NOT NULL DEFAULT current_timestamp(),
  `transaction_reference` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `reservations`
--

CREATE TABLE `reservations` (
  `reservation_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `showtime_id` int(11) NOT NULL,
  `reservation_date` timestamp NOT NULL DEFAULT current_timestamp(),
  `status` varchar(50) DEFAULT 'pending'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `reservations`
--

INSERT INTO `reservations` (`reservation_id`, `user_id`, `showtime_id`, `reservation_date`, `status`) VALUES
(3, 1, 1, '2026-03-03 08:19:58', 'confirmed'),
(4, 1, 1, '2026-03-03 08:30:51', 'confirmed'),
(5, 6, 1, '2026-03-04 07:14:25', 'confirmed'),
(7, 7, 1, '2026-03-04 07:31:29', 'confirmed'),
(8, 7, 2, '2026-03-04 08:07:57', 'confirmed');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `reservedseats`
--

CREATE TABLE `reservedseats` (
  `reserved_seat_id` int(11) NOT NULL,
  `reservation_id` int(11) NOT NULL,
  `seat_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `reservedseats`
--

INSERT INTO `reservedseats` (`reserved_seat_id`, `reservation_id`, `seat_id`) VALUES
(1, 3, 33),
(2, 3, 34),
(3, 4, 44),
(4, 4, 86),
(5, 4, 87),
(6, 4, 98),
(7, 4, 99),
(8, 5, 43),
(9, 7, 83),
(10, 8, 75);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `seats`
--

CREATE TABLE `seats` (
  `seat_id` int(11) NOT NULL,
  `hall_id` int(11) NOT NULL,
  `seat_number` varchar(10) NOT NULL,
  `is_vip` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `seats`
--

INSERT INTO `seats` (`seat_id`, `hall_id`, `seat_number`, `is_vip`) VALUES
(7, 1, 'A1', 0),
(8, 1, 'A2', 0),
(9, 1, 'A3', 0),
(10, 1, 'A4', 0),
(11, 1, 'A5', 0),
(12, 1, 'A6', 0),
(13, 1, 'A7', 0),
(14, 1, 'A8', 0),
(15, 1, 'A9', 0),
(16, 1, 'A10', 0),
(17, 1, 'A11', 0),
(18, 1, 'A12', 0),
(19, 1, 'A13', 0),
(20, 1, 'A14', 0),
(21, 1, 'A15', 0),
(22, 1, 'A16', 0),
(23, 1, 'A17', 0),
(24, 1, 'A18', 0),
(25, 1, 'A19', 0),
(26, 1, 'A20', 0),
(27, 1, 'A21', 0),
(28, 1, 'A22', 0),
(29, 1, 'A23', 0),
(30, 1, 'A24', 0),
(31, 1, 'A25', 0),
(32, 1, 'A26', 0),
(33, 1, 'A27', 0),
(34, 1, 'A28', 0),
(35, 1, 'A29', 0),
(36, 1, 'A30', 0),
(37, 1, 'A31', 0),
(38, 1, 'A32', 0),
(39, 1, 'A33', 0),
(40, 1, 'A34', 0),
(41, 1, 'A35', 0),
(42, 1, 'A36', 0),
(43, 1, 'A37', 0),
(44, 1, 'A38', 0),
(45, 1, 'A39', 0),
(46, 1, 'A40', 0),
(47, 1, 'A41', 0),
(48, 1, 'A42', 0),
(49, 1, 'A43', 0),
(50, 1, 'A44', 0),
(51, 1, 'A45', 0),
(52, 1, 'A46', 0),
(53, 1, 'A47', 0),
(54, 1, 'A48', 0),
(55, 1, 'A49', 0),
(56, 1, 'A50', 0),
(57, 1, 'A51', 0),
(58, 1, 'A52', 0),
(59, 1, 'A53', 0),
(60, 1, 'A54', 0),
(61, 1, 'A55', 0),
(62, 1, 'A56', 0),
(63, 1, 'A57', 0),
(64, 1, 'A58', 0),
(65, 1, 'A59', 0),
(66, 1, 'A60', 0),
(67, 1, 'A61', 0),
(68, 1, 'A62', 0),
(69, 1, 'A63', 0),
(70, 1, 'A64', 0),
(71, 1, 'A65', 0),
(72, 1, 'A66', 0),
(73, 1, 'A67', 0),
(74, 1, 'A68', 0),
(75, 1, 'A69', 0),
(76, 1, 'A70', 0),
(77, 1, 'A71', 0),
(78, 1, 'A72', 0),
(79, 1, 'A73', 0),
(80, 1, 'A74', 0),
(81, 1, 'A75', 0),
(82, 1, 'A76', 0),
(83, 1, 'A77', 0),
(84, 1, 'A78', 0),
(85, 1, 'A79', 0),
(86, 1, 'A80', 0),
(87, 1, 'A81', 0),
(88, 1, 'A82', 0),
(89, 1, 'A83', 0),
(90, 1, 'A84', 0),
(91, 1, 'A85', 0),
(92, 1, 'A86', 0),
(93, 1, 'A87', 0),
(94, 1, 'A88', 0),
(95, 1, 'A89', 0),
(96, 1, 'A90', 0),
(97, 1, 'A91', 0),
(98, 1, 'A92', 0),
(99, 1, 'A93', 0),
(100, 1, 'A94', 0),
(101, 1, 'A95', 0),
(102, 1, 'A96', 0),
(103, 1, 'A97', 0),
(104, 1, 'A98', 0),
(105, 1, 'A99', 0),
(106, 1, 'A100', 0),
(324, 2, 'B1', 0),
(325, 2, 'B2', 0),
(326, 2, 'B3', 0),
(327, 2, 'B4', 0),
(328, 2, 'B5', 0),
(329, 2, 'B6', 0),
(330, 2, 'B7', 0),
(331, 2, 'B8', 0),
(332, 2, 'B9', 0),
(333, 2, 'B10', 0),
(334, 2, 'B11', 0),
(335, 2, 'B12', 0),
(336, 2, 'B13', 0),
(337, 2, 'B14', 0),
(338, 2, 'B15', 0),
(339, 2, 'B16', 0),
(340, 2, 'B17', 0),
(341, 2, 'B18', 0),
(342, 2, 'B19', 0),
(343, 2, 'B20', 0),
(344, 2, 'B21', 0),
(345, 2, 'B22', 0),
(346, 2, 'B23', 0),
(347, 2, 'B24', 0),
(348, 2, 'B25', 0),
(349, 2, 'B26', 0),
(350, 2, 'B27', 0),
(351, 2, 'B28', 0),
(352, 2, 'B29', 0),
(353, 2, 'B30', 0),
(354, 2, 'B31', 0),
(355, 2, 'B32', 0),
(356, 2, 'B33', 0),
(357, 2, 'B34', 0),
(358, 2, 'B35', 0),
(359, 2, 'B36', 0),
(360, 2, 'B37', 0),
(361, 2, 'B38', 0),
(362, 2, 'B39', 0),
(363, 2, 'B40', 0),
(364, 2, 'B41', 0),
(365, 2, 'B42', 0),
(366, 2, 'B43', 0),
(367, 2, 'B44', 0),
(368, 2, 'B45', 0),
(369, 2, 'B46', 0),
(370, 2, 'B47', 0),
(371, 2, 'B48', 0),
(372, 2, 'B49', 0),
(373, 2, 'B50', 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `showtimes`
--

CREATE TABLE `showtimes` (
  `showtime_id` int(11) NOT NULL,
  `movie_id` int(11) NOT NULL,
  `hall_id` int(11) NOT NULL,
  `show_date` date NOT NULL,
  `show_time` time NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `showtimes`
--

INSERT INTO `showtimes` (`showtime_id`, `movie_id`, `hall_id`, `show_date`, `show_time`, `created_at`) VALUES
(1, 1, 1, '2025-12-05', '18:00:00', '2025-12-03 17:50:52'),
(2, 2, 2, '2025-12-05', '20:00:00', '2025-12-03 17:50:52');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `tickets`
--

CREATE TABLE `tickets` (
  `ticket_id` int(11) NOT NULL,
  `reservation_id` int(11) NOT NULL,
  `ticket_code` varchar(100) NOT NULL,
  `issued_date` timestamp NOT NULL DEFAULT current_timestamp(),
  `is_used` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `tickets`
--

INSERT INTO `tickets` (`ticket_id`, `reservation_id`, `ticket_code`, `issued_date`, `is_used`) VALUES
(4, 5, '796c6642-c977-43c6-9739-e654ca395f66', '2026-03-04 07:14:26', 0),
(5, 7, '8440c300-78f7-4ec6-9ea9-eb010b9eba90', '2026-03-04 07:31:29', 0),
(6, 8, 'c5458410-6453-4677-8d03-29fdeeb3ab32', '2026-03-04 08:07:58', 1);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `userroles`
--

CREATE TABLE `userroles` (
  `role_id` int(11) NOT NULL,
  `role_name` varchar(50) NOT NULL,
  `description` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `userroles`
--

INSERT INTO `userroles` (`role_id`, `role_name`, `description`) VALUES
(1, 'Admin', 'Rendszergazda'),
(2, 'Customer', 'Vásárló');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `users`
--

CREATE TABLE `users` (
  `user_id` int(11) NOT NULL,
  `role_id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `password_hash` varchar(255) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`user_id`, `role_id`, `name`, `email`, `phone`, `password_hash`, `created_at`) VALUES
(1, 1, 'Kovács János', 'janos.kovacs@example.com', '06701234567', 'hash1', '2025-12-03 17:50:52'),
(2, 2, 'Nagy Éva', 'eva.nagy@example.com', '06707654321', 'hash2', '2025-12-03 17:50:52'),
(3, 2, 'steam', 'dinoforstea@gmail.com', NULL, 'szia', '2026-01-15 07:00:44'),
(6, 1, 'te a', 'tekulicsb@kkszki.hu', '+36132351531', '$2a$11$6JD0xed97hEDlQVt52NhRewY/yAiWh7v7WdMFEwSeBENIXI.Uy5Sm', '2026-03-03 07:54:48'),
(7, 1, 'dasda', 'tothr@kkszki.hu', '3232131231', '$2a$11$ieiZY7dfsDaIxEOw471TYOvMRSj1sU7k0YPDLyRkgiAKwZ.lS7p4m', '2026-03-03 09:50:34');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `user_verifications`
--

CREATE TABLE `user_verifications` (
  `id` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `code` varchar(6) NOT NULL,
  `expires_at` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `emaillogs`
--
ALTER TABLE `emaillogs`
  ADD PRIMARY KEY (`email_log_id`),
  ADD KEY `user_id` (`user_id`);

--
-- A tábla indexei `halls`
--
ALTER TABLE `halls`
  ADD PRIMARY KEY (`hall_id`);

--
-- A tábla indexei `movies`
--
ALTER TABLE `movies`
  ADD PRIMARY KEY (`movie_id`);

--
-- A tábla indexei `payments`
--
ALTER TABLE `payments`
  ADD PRIMARY KEY (`payment_id`),
  ADD KEY `reservation_id` (`reservation_id`);

--
-- A tábla indexei `reservations`
--
ALTER TABLE `reservations`
  ADD PRIMARY KEY (`reservation_id`),
  ADD KEY `user_id` (`user_id`),
  ADD KEY `showtime_id` (`showtime_id`);

--
-- A tábla indexei `reservedseats`
--
ALTER TABLE `reservedseats`
  ADD PRIMARY KEY (`reserved_seat_id`),
  ADD UNIQUE KEY `reservation_id` (`reservation_id`,`seat_id`),
  ADD KEY `seat_id` (`seat_id`);

--
-- A tábla indexei `seats`
--
ALTER TABLE `seats`
  ADD PRIMARY KEY (`seat_id`),
  ADD UNIQUE KEY `hall_id` (`hall_id`,`seat_number`);

--
-- A tábla indexei `showtimes`
--
ALTER TABLE `showtimes`
  ADD PRIMARY KEY (`showtime_id`),
  ADD UNIQUE KEY `movie_id` (`movie_id`,`hall_id`,`show_date`,`show_time`),
  ADD KEY `hall_id` (`hall_id`);

--
-- A tábla indexei `tickets`
--
ALTER TABLE `tickets`
  ADD PRIMARY KEY (`ticket_id`),
  ADD UNIQUE KEY `ticket_code` (`ticket_code`),
  ADD KEY `reservation_id` (`reservation_id`);

--
-- A tábla indexei `userroles`
--
ALTER TABLE `userroles`
  ADD PRIMARY KEY (`role_id`),
  ADD UNIQUE KEY `role_name` (`role_name`);

--
-- A tábla indexei `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`user_id`),
  ADD UNIQUE KEY `email` (`email`),
  ADD KEY `role_id` (`role_id`);

--
-- A tábla indexei `user_verifications`
--
ALTER TABLE `user_verifications`
  ADD PRIMARY KEY (`id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `emaillogs`
--
ALTER TABLE `emaillogs`
  MODIFY `email_log_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT a táblához `halls`
--
ALTER TABLE `halls`
  MODIFY `hall_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT a táblához `movies`
--
ALTER TABLE `movies`
  MODIFY `movie_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `payments`
--
ALTER TABLE `payments`
  MODIFY `payment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT a táblához `reservations`
--
ALTER TABLE `reservations`
  MODIFY `reservation_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT a táblához `reservedseats`
--
ALTER TABLE `reservedseats`
  MODIFY `reserved_seat_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT a táblához `seats`
--
ALTER TABLE `seats`
  MODIFY `seat_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=387;

--
-- AUTO_INCREMENT a táblához `showtimes`
--
ALTER TABLE `showtimes`
  MODIFY `showtime_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT a táblához `tickets`
--
ALTER TABLE `tickets`
  MODIFY `ticket_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT a táblához `userroles`
--
ALTER TABLE `userroles`
  MODIFY `role_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT a táblához `user_verifications`
--
ALTER TABLE `user_verifications`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `emaillogs`
--
ALTER TABLE `emaillogs`
  ADD CONSTRAINT `emaillogs_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`);

--
-- Megkötések a táblához `payments`
--
ALTER TABLE `payments`
  ADD CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`reservation_id`) REFERENCES `reservations` (`reservation_id`) ON DELETE CASCADE;

--
-- Megkötések a táblához `reservations`
--
ALTER TABLE `reservations`
  ADD CONSTRAINT `reservations_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`),
  ADD CONSTRAINT `reservations_ibfk_2` FOREIGN KEY (`showtime_id`) REFERENCES `showtimes` (`showtime_id`);

--
-- Megkötések a táblához `reservedseats`
--
ALTER TABLE `reservedseats`
  ADD CONSTRAINT `reservedseats_ibfk_1` FOREIGN KEY (`reservation_id`) REFERENCES `reservations` (`reservation_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `reservedseats_ibfk_2` FOREIGN KEY (`seat_id`) REFERENCES `seats` (`seat_id`);

--
-- Megkötések a táblához `seats`
--
ALTER TABLE `seats`
  ADD CONSTRAINT `seats_ibfk_1` FOREIGN KEY (`hall_id`) REFERENCES `halls` (`hall_id`);

--
-- Megkötések a táblához `showtimes`
--
ALTER TABLE `showtimes`
  ADD CONSTRAINT `showtimes_ibfk_1` FOREIGN KEY (`movie_id`) REFERENCES `movies` (`movie_id`),
  ADD CONSTRAINT `showtimes_ibfk_2` FOREIGN KEY (`hall_id`) REFERENCES `halls` (`hall_id`);

--
-- Megkötések a táblához `tickets`
--
ALTER TABLE `tickets`
  ADD CONSTRAINT `tickets_ibfk_1` FOREIGN KEY (`reservation_id`) REFERENCES `reservations` (`reservation_id`);

--
-- Megkötések a táblához `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `users_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `userroles` (`role_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
