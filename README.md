# Multipart Serializer	

This is a dot net standard library that serializes a C# object to a MultipartFormDataContent object that can be used by HttpClient Post. The resulting object will be assigned to the HttpContent base class.

## Usage

To serialize your objects to MultipartFormDataContent, use the following syntax:

```c#
var formData = TiberHealth.Serializer.FormDataSerializer.FormDataContent(obj);
```

```c#
var dataObject = new dataObject();
var formData = TiberHealth.Serializer.FormDataSerializer.FormDataContent(dataObject);
```

formData will be of type MultipartFormDataContent.

## Serialization

Currently all part will be one of the following content types:

### StringContent

Represents the data as a string. This is the default type if the other types are not satisfied. The name of the data part will take the name of the field, unless supplied a name using the Multipart Attribute.

### ByteArrayContent

Data that is in a byte array will be serialized into the ByteArrayContent. The name of the form-part will be the name of the property, unless overridden using the Multipart or MultipartFile attributes.  The MuiltipartFile attribute can be used to identify the ContentType and the Filename. 

## Property Class Serialization

If the property is a class structure, the serializer will default to a StringContent and serialize the property into JSON. The Multipart attribute can be use to signal which field of the class should be used as the value instead of serialization. The MultipartFile attribute identifies that class as a file container and uses it parameters to build a ByteArrayContent representing and attaching the byte array as a file, setting the filename and content type. 

## [Multipart] Attribute

The Mutipart attribute is used for two primary purposes.

### Name Overriding

The first purpose is to override the name of the form-data part.  

```c#
[TiberHealth.Serializer.Multipart(Name = "LocalState")]
public string State => "NC";
```
The above code will serialize the ***State*** field as ***LocalState*** in the content. 

**Note =>** *You can also rename a field using the **JsonPropertyName** from System.Text.Json.Serialization or the **JsonProperty** from Newtonsoft.Json to name the field for serialization.*

### Class Value Identification

The second purpose of the Multipart Attribute is to identfy the data field for a property class. 
```c#
public class serialableObject 
{
     [Multipart(Name="CustomerGuid", Value="ObjectGuid")]
     public Customer Customer { get; set;}
}
var formData = TiberHealth.Serializer.FormDataserializer.FormDataContent(serialableObject);
```

The above code will create a StringContent part called CustomerGuid and place the ObjectGuid value from the Customer record for its value. 

### Enum Behavior

Another purpose of the Multipart field is to identify if an Enum should be represented by it value or the enum field name (string). 

***The default behavior is for Enums to be displayed as its value***

The Multipart attribute can be added directly to the Enum for global usage

```c#
[Multipart(EnumAsString = true)]
internal enum TestStringEnum
{
  testValue1 = 1,
  testValue2 = 2,
  [EnumSerializedValue("peterpan")]testValue3 = 4
}
```

or it can be added to the field using the Enum for independant behavior.  

```
[Multipart(EnumAsString = true)] public TestEnum EnumAsString => TestEnum.value3;
```
The ***EnumSerializedValue*** attribute overrides the string value for an Enum when marked as *EnumAsString=true*.

## [MultipartFile] Attribute

The MultiPartFile attribute is to identify the name, filename and content type of the file to be used for creating the content diposition of the content part. 

```c#
[MultipartFile(FileName = "FileName", ContentType="ContentType", Value="FileBytes", Name="File")]
```
**File Name** Identifies either the constant name for the file name or the property in the class that contains the file name. If the property does not exist, the serializer assumes that the value is a constant. If FileName is not provided the serializer defaults to FileName. 
**ContentType** identifies either the constant value oor the property that identifies the content type of the file being attached. The content type must be a standard content type that is known to *MediaTypeHeaderValue*. 

**Value** and **Name** are inherited from the Multipart attribute. 

**Value** is required for MultiPartFile and must be of type byte[] (byte array).

***Note =>*** It is planned for next release to allow a byte stream to be used as well as a byte array.

## [MultipartIgnore] Attribute

The MultipartIgnore attribute is used to identify which properties should be ignored for serialization.  the JsonIgnore from both Newtonsoft.Json and System.Text.Json.Serialization also signal the serializer to not serialize the property. 

## [MultipartInclude] Attribute

The MultipartIncludeore attribute is used to identify which properties should be included for serialization that are marked with the JsonIgnore atttribute.  The attribute is used for when the property should be included with the multipart form data, but will not be included in a JSON serialization. To ignore in the JSON serialization, the JsonIgnore attribute is still required.  

## Futher Development, Requests and issues

Feel free to open Pull Requests or send us any enhancement requests and issues through GitHub. 

