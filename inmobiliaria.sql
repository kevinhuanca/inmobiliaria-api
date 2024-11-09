-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 10-11-2024 a las 00:25:52
-- Versión del servidor: 10.4.27-MariaDB
-- Versión de PHP: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contratos`
--

CREATE TABLE `contratos` (
  `Id` int(11) NOT NULL,
  `Monto` int(11) NOT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL,
  `InquilinoId` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contratos`
--

INSERT INTO `contratos` (`Id`, `Monto`, `FechaInicio`, `FechaFin`, `InquilinoId`, `InmuebleId`) VALUES
(1, 130000, '2024-11-03', '2025-02-06', 2, 2),
(2, 120000, '2024-10-07', '2024-12-19', 1, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmuebles`
--

CREATE TABLE `inmuebles` (
  `Id` int(11) NOT NULL,
  `Direccion` varchar(50) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `Tipo` varchar(50) NOT NULL,
  `Uso` varchar(50) NOT NULL,
  `Precio` int(11) NOT NULL,
  `Disponible` tinyint(1) NOT NULL,
  `Imagen` varchar(50) NOT NULL,
  `PropietarioId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmuebles`
--

INSERT INTO `inmuebles` (`Id`, `Direccion`, `Ambientes`, `Tipo`, `Uso`, `Precio`, `Disponible`, `Imagen`, `PropietarioId`) VALUES
(1, 'San Martin 213', 2, 'Casa', 'Residencial', 120900, 1, 'in97f0a2c9-a5a5-4e00-bde9-5dd0e11be4d4.jpg', 2),
(2, 'Ayacucho 450', 4, 'Casa', 'Residencial', 133700, 1, 'in8dd38623-bf36-40c8-836e-f152415768b5.jpg', 2),
(3, 'Rivadavia 123', 3, 'Local', 'Comercial', 123123, 0, 'in0f9515f4-b019-4c71-870a-5aaec6e1d22e.jpg', 2),
(4, 'La Finur 555', 4, 'Casa', 'Residencial', 432432, 0, 'inb8320a85-f84d-415c-9d34-499eb27383e7.jpg', 2),
(5, 'Lavalle 432', 5, 'Oficina', 'Comercial', 600900, 0, 'in5337b709-e4b4-4152-b789-532c4fb6f72b.jpg', 1),
(7, 'Centenario 423', 4, 'Casa', 'Residencial', 123124, 1, 'in0b9362a4-40d5-44e8-a467-82dcf1f1eac7.jpg', 2);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilinos`
--

CREATE TABLE `inquilinos` (
  `Id` int(11) NOT NULL,
  `Dni` varchar(50) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Telefono` varchar(50) NOT NULL,
  `Email` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilinos`
--

INSERT INTO `inquilinos` (`Id`, `Dni`, `Nombre`, `Apellido`, `Telefono`, `Email`) VALUES
(1, '43092121', 'Mario', 'Guti', '2652908790', 'mg@gmail.com'),
(2, '44223234', 'Carlos', 'Sainz', '765456345', 'cs@gmail.com'),
(3, '90876678', 'Sofia', 'Garay', '2665678767', 'sg@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pagos`
--

CREATE TABLE `pagos` (
  `Id` int(11) NOT NULL,
  `Numero` int(11) NOT NULL,
  `Fecha` date NOT NULL,
  `Importe` int(11) NOT NULL,
  `ContratoId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pagos`
--

INSERT INTO `pagos` (`Id`, `Numero`, `Fecha`, `Importe`, `ContratoId`) VALUES
(1, 1, '2024-11-05', 130000, 1),
(2, 2, '2024-11-05', 130000, 1),
(3, 1, '2024-10-01', 120000, 2),
(4, 2, '2024-10-01', 120000, 2),
(5, 3, '2024-10-01', 120000, 2);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietarios`
--

CREATE TABLE `propietarios` (
  `Id` int(11) NOT NULL,
  `Dni` varchar(50) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Telefono` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Clave` varchar(100) NOT NULL,
  `Avatar` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietarios`
--

INSERT INTO `propietarios` (`Id`, `Dni`, `Nombre`, `Apellido`, `Telefono`, `Email`, `Clave`, `Avatar`) VALUES
(1, '32132131', 'Juan', 'Perez', '2664323232', 'jp@gmail.com', 'wWJ9DkPtMbNyaZobc+pnspuVsT4OY6+mZhoWbnu2tcs=', 'av342add8d-bf02-4493-bd9b-3dee7a7a7222.jpg'),
(2, '4982332', 'Marias', 'Perez', '2664123123', 'mp@gmail.com', 'wWJ9DkPtMbNyaZobc+pnspuVsT4OY6+mZhoWbnu2tcs=', 'av93c5340f-f896-4d24-8c71-8326d935280a.jpg');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InquilinoId` (`InquilinoId`),
  ADD KEY `InmuebleId` (`InmuebleId`);

--
-- Indices de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PropietarioId` (`PropietarioId`);

--
-- Indices de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `ContratoId` (`ContratoId`);

--
-- Indices de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contratos`
--
ALTER TABLE `contratos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `inquilinos`
--
ALTER TABLE `inquilinos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de la tabla `pagos`
--
ALTER TABLE `pagos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `propietarios`
--
ALTER TABLE `propietarios`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contratos`
--
ALTER TABLE `contratos`
  ADD CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilinos` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`InmuebleId`) REFERENCES `inmuebles` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `inmuebles`
--
ALTER TABLE `inmuebles`
  ADD CONSTRAINT `inmuebles_ibfk_1` FOREIGN KEY (`PropietarioId`) REFERENCES `propietarios` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Filtros para la tabla `pagos`
--
ALTER TABLE `pagos`
  ADD CONSTRAINT `pagos_ibfk_1` FOREIGN KEY (`ContratoId`) REFERENCES `contratos` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
