# Краткое руководство

## Основная информация

Блоки кода имеют общий вид (пробелы необязательны)

     {{ <i>name</i> [( <i>expression</i> )] [ begin | continue | end ] }} | {{ <i>expression</i> }}

где:

* <i>name</i> -  название блока кода

* <i>expression</i> - корректное выражение с точки зрения синтаксиса C#.

## Блоки

### Блок выражения

Синтаксис:

     {{ <i>expression</i> }}

Например:

    var template = @"{{ Name }} is simply dummy text of the printing and typesetting industry. {{ Name }} has been the industry's standard dummy text ever since the {{ Date.Year }}s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the {{ Date.Year + 460 }}s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of {{ Name }}.";
     
     Console.WriteLine(new HtmlEngineService().GetHtml(template, new TestValue() { Name = "Lorem Ipsum", Date = new DateTime(1500, 1, 1) }));
     //Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.

### Блок if

Синтаксис:

     {{ if [( <i>expression</i> )] begin }}
     [{{ elseif [( <i>expression</i> )] continue }}]
     ...
     [{{ elseif [( <i>expression</i> )] continue }}]
     [{{ else continue }}]
     {{ if end }}

Например:

     var template = @"{{ if (x < 1) begin }}
         x less than 1
     {{ elseif (x > 0 && x < 5) continue }}
         x between 0 and 5
     {{ elseif (x > 4 && x < 10) continue }}
         x between 4 and 10
     {{ elseif (x % 2 == 0) continue }}
         x is even
     {{ else continue }}
         x is x
     {{ if end }}";
     Console.WriteLine(new HtmlEngineService().GetHtml(template, new TestIf() { x = 5 }));
     //    x between 4 and 10
     Console.WriteLine(new HtmlEngineService().GetHtml(template, new TestIf() { x = 20 }));
     //    x is even

Важные уточнения: 

* Отсутствие скобок с выражением эквивалентно выражению true.

* Названия continue блоков может быть любым или вовсе отсутствовать (т.е. блоки 

>     {{ elseif (5 == 5) continue }}, 
>     {{ else continue }}, 
>     {{ continue }}, 
>     {{ (true) continue }}, 
>     {{ Lorem Ipsum is simply dummy text... continue }} 
>
> эквивалентны). В примере имена else и elseif даны лишь для удобства.

### Блок foreach

Синтаксис:

     {{ foreach ( <i>variable</i> in <i>enumerable</i> ) begin }}
     {{ foreach end }}

Например:

     var template = @"{{ foreach ( number in numbers ) begin }}
         {{ if (number > 0) begin }}
             number {{number}} is positive
         {{ if end }}
     {{ foreach end }}";
     Console.WriteLine(new HtmlEngineService().GetHtml(template, new TestForeach() { numbers = new[] { 1, 2, -3, 4 } }));
     //        number 1 is positive
     //        number 2 is positive
     //        number 4 is positive

Важные уточнения: 

* Название переменной <i>variable</i> должно быть корректным с точки зрения синтаксиса C#.

* Очевидно, что <i>enumerable</i> должно являтся перечислимым типом.

## Применение

Для создания текста по шаблону необходимо:

* Создать экземпляр класса HtmlEngineService() из пространства имен HtmlEngineLibrary.
* Вызвать метод с нужными типами параметров, подав на вход шаблон в виде строки и экземпляр какого-либо класса в виде модели (свойства этого класса можно использовать в выражениях)