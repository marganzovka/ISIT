﻿// Создание кодовой таблицы сжатия информации методом Шеннона-Фaно

type
  Символ = record
    Байт: byte;
    Част: real;
    Код := ''
  end;

var
  ИнфОБайте := new Символ[256];

{1 Рекурсивная процедура реализующая метод Шеннона-Фано}
procedure Шеннон_Фано(Начало, Конец: integer);
begin
  if Начало < Конец then begin

    {2 Вычисление полусуммы частот в заданном диапазоне}
    var ПолуСуммЧаст := 0.0;
    for var M := Начало to Конец do ПолуСуммЧаст += ИнфОБайте[M].Част;
    ПолуСуммЧаст /= 2;

    {3 Нахождение границы разделения по частотам}
    var Граница := Начало - 1;
    var НакоплСумм := 0.0;
    repeat
      inc(Граница);
      НакоплСумм += ИнфОБайте[Граница].Част;
    until (НакоплСумм > ПолуСуммЧаст);

    {4 Корректировка границы разделения}
    if Граница > Начало then
      if abs(НакоплСумм - ПолуСуммЧаст) > abs(НакоплСумм - ПолуСуммЧаст - ИнфОБайте[Граница].Част)
      then dec(Граница);

    {5 Присвоение кода символам в разделенных диапазонах}
    for var M := Начало to Граница do ИнфОБайте[M].Код := ИнфОБайте[M].Код + '0';
    for var M := Граница + 1 to Конец do ИнфОБайте[M].Код := ИнфОБайте[M].Код + '1';

    {6 Рекурсивные вызовы для обработки двух полученных диапазонов}
    Шеннон_Фано(Начало, Граница);
    Шеннон_Фано(Граница + 1, Конец);

  end

end;

begin
  var ИмяФ := ReadlnString('Введите имя файла с частотной таблицей -> ');

  {7 Открытие файловых потоков для чтения и записи}
  var ИсхФ, РезФ : text;
  Reset(ИсхФ, ИмяФ);
  Rewrite(РезФ, ИмяФ[:LastPos('.', ИмяФ)] + '.S-F');
  
  {8 Чтение размера файла и энтропии, запись в результирующий файл}
  var ДлФ := ИсхФ.ReadlnInteger;
  РезФ.Writeln(ДлФ, #9, 'Размер файла');
  РезФ.Writeln(ИсхФ.ReadlnReal, #9, 'Энтропия');
  ИсхФ.Readln; ИсхФ.Readln;

  {9 Заполнение массива символов данными из файла}
  var МаксИнд := -1;
  repeat
    inc(МаксИнд);
    Readln(ИсхФ, ИнфОБайте[МаксИнд].Байт, ИнфОБайте[МаксИнд].Част);
  until ИсхФ.Eof;
  ИсхФ.Close;

  {10 Вызов процедуры Шеннон_Фано для построения кодовой таблицы}
  Шеннон_Фано(0, МаксИнд);

  {11 Сортировка кодовой таблицы по длине кодов}
  var Усл : boolean;
  repeat
    Усл := True;
    for var i := 0 to МаксИнд - 1 do
      If length(ИнфОБайте[i].Код) > length(ИнфОБайте[i + 1].Код) then begin
        swap(ИнфОБайте[i].Код, ИнфОБайте[i + 1].Код);
        Усл := False
      end;
  until Усл;
  
  {12 Вычисление средней длины кода и запись в результирующий файл}
  var СрДлК := 0.0;
  for var I := 0 to МаксИнд do
    СрДлК += length(ИнфОБайте[I].Код) * ИнфОБайте[I].Част;
  РезФ.Writeln(СрДлК / ДлФ, #9, 'Средняя длина кода'); РезФ.Writeln;

  {13 Запись кодовой таблицы в результирующий файл}
  РезФ.Writeln('Байт', #9, 'Код');
  for var I := 0 to МаксИнд do
    РезФ.Writeln(ИнфОБайте[I].Байт, #9, ИнфОБайте[I].Код);

  РезФ.Close

end.