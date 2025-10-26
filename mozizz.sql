-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Okt 26. 13:46
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.0.30

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
  `release_date` date DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `reservedseats`
--

CREATE TABLE `reservedseats` (
  `reserved_seat_id` int(11) NOT NULL,
  `reservation_id` int(11) NOT NULL,
  `seat_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `tickets`
--

CREATE TABLE `tickets` (
  `ticket_id` int(11) NOT NULL,
  `reservation_id` int(11) NOT NULL,
  `ticket_code` varchar(100) NOT NULL,
  `issued_date` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `userroles`
--

CREATE TABLE `userroles` (
  `role_id` int(11) NOT NULL,
  `role_name` varchar(50) NOT NULL,
  `description` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `emaillogs`
--
ALTER TABLE `emaillogs`
  MODIFY `email_log_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `halls`
--
ALTER TABLE `halls`
  MODIFY `hall_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `movies`
--
ALTER TABLE `movies`
  MODIFY `movie_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `payments`
--
ALTER TABLE `payments`
  MODIFY `payment_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `reservations`
--
ALTER TABLE `reservations`
  MODIFY `reservation_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `reservedseats`
--
ALTER TABLE `reservedseats`
  MODIFY `reserved_seat_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `seats`
--
ALTER TABLE `seats`
  MODIFY `seat_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `showtimes`
--
ALTER TABLE `showtimes`
  MODIFY `showtime_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `tickets`
--
ALTER TABLE `tickets`
  MODIFY `ticket_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `userroles`
--
ALTER TABLE `userroles`
  MODIFY `role_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT;

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
