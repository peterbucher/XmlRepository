# XmlRepository

## History

*NOTE: Please note that XmlRepository is a project that was created in 2011, but has been abandoned since then. Hence it may not represent our current knowledge and best practices. This repository is basically there for historical reasons and for the case that anyone might be interested in the old implementation. Feel free to fork and send pull requests.*

```C#
<xmlrepository.ch>
    <description><![CDATA[
        XML-basiertes, threadsicheres Repository für flache .NET-Objekte, welches
        das Dateisystem und In-Memory unterstützt.
    ]]></description>

    <example><![CDATA[
        XmlRepository.DefaultQueryProperty = "Id";
        XmlRepository.DataProvider = new XmlFileProvider("~/App_Data/");

        using(var repository = XmlRepository.GetInstance<Foo>) {
            var foos = repository.LoadAllBy(f => f.Id > 42);
            repository.SaveOnSubmit(new Foo { Id = 27, ... });
            repository.DeleteOnSubmit(f => f.Id == 23);
        }
    ]]></example>

    <downloads>
        <download file="assembly" version="1.1" platform="net35" type="application/zip" />
        <download file="source" version="current" platform="net35" type="git" />
    </downloads>

    <documentation />

    <copyright><![CDATA[
        © Copyright 2009 Golo Roden und Peter Bucher. Alle Rechte vorbehalten.
    ]]></copyright>
</xmlrepository.ch>
```
