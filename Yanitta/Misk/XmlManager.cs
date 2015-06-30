﻿using System;
using System.IO;
using System.Xml.Serialization;

namespace Yanitta
{
    /// <summary>
    /// Представляет объект для сериализации/десериализации объекта в файл/из файла.
    /// </summary>
    public static class XmlManager
    {
        /// <summary>
        /// Десериализирует файл в объект.
        /// </summary>
        /// <typeparam name="T">Тип объекта для десериализации.</typeparam>
        /// <param name="path">Файл который надо десериализовать.</param>
        /// <returns>Десериализированый объект.</returns>
        public static T Load<T>(string path) where T : class
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File not found", path);

            using (var fstream = File.OpenRead(path))
            {
                var serialiser = new XmlSerializer(typeof(T));

                serialiser.UnknownAttribute += (o, e) => {
                    Console.WriteLine("Unknown attribute: {0} at line: {1} position: {2}",
                        e.Attr, e.LineNumber, e.LinePosition);
                };

                serialiser.UnknownElement += (o, e) => {
                    Console.WriteLine("Unknown Element: {0} at line: {1} position: {2}",
                        e.Element, e.LineNumber, e.LinePosition);
                };

                return (T)serialiser.Deserialize(fstream);
            }
        }

        /// <summary>
        /// Сериализирует объект в файл.
        /// </summary>
        /// <typeparam name="T">Тип объекта для сериализации.</typeparam>
        /// <param name="path">Файл который надо сериализовать.</param>
        /// <param name="obj">Объект для сериализации.</param>
        public static void Save<T>(string path, T obj) where T : class
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path");

            if (obj == null)
                throw new ArgumentNullException("obj");

            using (var fstream = File.Open(path, FileMode.Create))
            {
                var serialiser = new XmlSerializer(typeof(T));
                serialiser.Serialize(fstream, obj);
                fstream.Flush();
            }
        }
    }
}