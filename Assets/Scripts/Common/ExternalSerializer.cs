using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace Momentum {

	/// <summary>
	/// Encapsulates the process of reading and writing objects into a specified folder.
	/// </summary>
	public class ExternalSerializer {

		// ================
		// Public Interface
		// ================

		/// <summary>
		/// The working folder that this serializer reads and writes to.
		/// 
		/// <para>
		/// This cannot be reassigned because ExternalSerializer guarantees
		/// that the directory exists if the constructor is successful.
		/// </para>
		/// 
		/// </summary>
		public string BasePath { get; }

		/// <summary>
		/// Creates a new serializer and sets where the serializer will read and write to.
		/// 
		/// <para>
		/// If the folder does not exist, it will be created. If the folder
		/// exists, the constructor will leave its contents untouched.
		/// </para>
		/// 
		/// </summary>
		/// <param name="path">Path to working folder.</param>
		public ExternalSerializer(string path)
		{
			BasePath = Path.Combine(path);
			Directory.CreateDirectory(BasePath);
		}

		/// <summary>
		/// Returns true if the given file exists in the working folder.
		/// </summary>
		/// <param name="filename">Name of the file</param>
		public bool Exists(string filename)
		{
			return File.Exists(GetPath(filename));
		}

		/// <summary>
		/// Delete a file in the working folder.
		/// 
		/// <para>
		/// If the operation fails for any reason, this function rethrows the
		/// exception to the caller.
		/// </para>
		/// 
		/// </summary>
		/// <param name="filename">Name of the file.</param>
		public void Delete(string filename)
		{
			File.Delete(GetPath(filename));
		}

		/// <summary>
		/// Serialize an object to a file in the working folder, replacing an existing file if it exists.
		/// 
		/// <para>
		/// This function gives the strong guarantee. If serialization fails
		/// for any reason, then the directory will be left exactly like it was
		/// before the operation started. Upon success, there will be a
		/// new/updated file in the directory that contains the contents of the
		/// serialized object.
		/// </para>
		/// 
		/// <para>
		/// Rethrows any exceptions that occur during the process.
		/// </para>
		/// 
		/// </summary>
		/// <typeparam name="T">The type of the object to serialize.</typeparam>
		/// <param name="item">Object to serialize.</param>
		/// <param name="filename">Name of the file to serialize the object to.</param>
		public void Serialize<T>(T item, string filename)
		{
			if (item == null) {
				throw new ArgumentNullException("ExternalSerializer: Cannot serialize null object.");
			}

			string finalPath = GetPath(filename);
			string tmpPath = finalPath + TempSuffix;

			try {
				using (StreamWriter sw = new StreamWriter(tmpPath))
				using (JsonWriter writer = new JsonTextWriter(sw)) {
					try {
						DefaultSerializer.Serialize(writer, item);
					} catch (JsonException e) {
						// Hide JSON implementation details.
						throw new Exception(e.Message);
					}
				}
			} catch {
				File.Delete(finalPath);
				throw;
			}

			try {
				File.Delete(finalPath);
				File.Move(tmpPath, finalPath);
			} catch {
				File.Delete(finalPath);
				throw;
			}
		}

		/// <summary>
		/// Deserialize an object from a file in the working folder.
		/// 
		/// <para>
		/// Rethrows any exceptions that occur during deserialization.
		/// </para>
		/// 
		/// </summary>
		/// <typeparam name="T">Type of the object being deserialized.</typeparam>
		/// <param name="filename">Name of the file holding the serialized object.</param>
		/// <returns>An object reconstructed from the data in the file.</returns>
		public T Deserialize<T>(string filename)
		{
			using (StreamReader sr = new StreamReader(GetPath(filename)))
			using (JsonReader reader = new JsonTextReader(sr)) {
				try {
					return DefaultSerializer.Deserialize<T>(reader);
				} catch (JsonException e) {
					// Hide JSON implementation details.
					throw new Exception(e.Message);
				}
			}
		}

		// ======================
		// Private Implementation
		// ======================

		private const string TempSuffix = ".tmp~";
		private static readonly JsonSerializer DefaultSerializer = new JsonSerializer() {
			Formatting = Formatting.Indented,
		};

		static ExternalSerializer()
		{
			DefaultSerializer.Converters.Add(new Convert_Vector2());
			DefaultSerializer.Converters.Add(new Convert_Vector3());
			DefaultSerializer.Converters.Add(new Convert_Vector4());
			DefaultSerializer.Converters.Add(new Convert_Quaternion());
		}

		#region Unity Type Converters

		// None of these types can be serialized using the default Json serializer without throwing an exception.

		private class Convert_Vector2 : JsonConverter<Vector2> {
			public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var v = new Vector2(
					(float)reader.ReadAsDouble(),
					(float)reader.ReadAsDouble());
				reader.Read();
				return v;
			}

			public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
			{
				writer.WriteStartArray();
				writer.WriteValue(value.x);
				writer.WriteValue(value.y);
				writer.WriteEndArray();
			}
		}

		private class Convert_Vector3 : JsonConverter<Vector3> {
			public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var v = new Vector3(
					(float)reader.ReadAsDouble(),
					(float)reader.ReadAsDouble(),
					(float)reader.ReadAsDouble());
				reader.Read();
				return v;
			}

			public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
			{
				writer.WriteStartArray();
				writer.WriteValue(value.x);
				writer.WriteValue(value.y);
				writer.WriteValue(value.z);
				writer.WriteEndArray();
			}
		}

		private class Convert_Vector4 : JsonConverter<Vector4> {
			public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				var v = new Vector4(
					(float)reader.ReadAsDouble(),
					(float)reader.ReadAsDouble(),
					(float)reader.ReadAsDouble(),
					(float)reader.ReadAsDouble());
				reader.Read();
				return v;
			}

			public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
			{
				writer.WriteStartArray();
				writer.WriteValue(value.x);
				writer.WriteValue(value.y);
				writer.WriteValue(value.z);
				writer.WriteValue(value.w);
				writer.WriteEndArray();
			}
		}

		private class Convert_Quaternion : JsonConverter<Quaternion> {
			public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				Quaternion q = new Quaternion(
					(float)reader.ReadAsDouble(),
					(float)reader.ReadAsDouble(),
					(float)reader.ReadAsDouble(),
					(float)reader.ReadAsDouble());
				reader.Read();
				return q;
			}

			public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
			{
				writer.WriteStartArray();
				writer.WriteValue(value.x);
				writer.WriteValue(value.y);
				writer.WriteValue(value.z);
				writer.WriteValue(value.w);
				writer.WriteEndArray();
			}
		}

		#endregion

		private string GetPath(string add)
		{
			if (add == null) {
				throw new ArgumentNullException("ExternalSerializer: Filename cannot be null");
			}
			return Path.Combine(BasePath, add);
		}
	}

}
