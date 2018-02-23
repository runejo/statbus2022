TRUNCATE TABLE [dbo].[ReorgTypes]

INSERT INTO ReorgTypes
  (Name, Code, IsDeleted)
VALUES
  ('Создание юридического лица без обособленных подразделений', '10', 0),
  ('Создание нового юридического лица', '11', 0),
  ('Создание в результате слияния', '12', 0),
  ('Создание в результате разделения (раскола)', '13', 0),
  ('Создание в результате выделения (уступка)', '14', 0),
  ('Создание в результате преобразования', '15', 0),
  ('Прекращение деятельности юридического лица без обособленных подразделений', '20', 0),
  ('Прекращение деятельности в результате слияния', '21', 0),
  ('Прекращение деятельности в результате разделения (раскола)', '22', 0),
  ('Прекращение деятельности в результате присоединения одной единицы к другой', '23', 0),
  ('Прекращение деятельности в результате преобразования', '24', 0),
  ('Ликвидация юридического лица', '30', 0),
  ('Ликвидация по решению учредителей (участников)', '31', 0),
  ('Ликвидация по решению суда недействительной регистрации', '32', 0),
  ('Ликвидация вследствии аннулирования лицензии', '33', 0),
  ('Ликвидация платежеспособного и неплатежеспособного предприятия без участия суда (временный ликвидатор)', '34', 0),
  ('Ликвидация неплатежеспособного предприятия с участием суда', '35', 0),
  ('Ликвидация вследствие отмены регистрации регистрирующим органом без участия суда', '36', 0),
  ('Ликвидация по решению суда', '37', 0),
  ('Создание обособленного подразделения, связанного с одним юридическим лицом', '40', 0),
  ('Создание обособленного подразделения', '41', 0),
  ('Создание в результате слияния', '42', 0),
  ('Создание в результате разделения (раскола)', '43', 0),
  ('Создание в результате выделения (уступка)', '44', 0),
  ('Создание в результате преобразования', '45', 0),
  ('Прекращение деятельности обособленного подразделения, связанного с одним юридическим лицом', '50', 0),
  ('Прекращение деятельности обособленного подразделения', '51', 0),
  ('Прекращение деятельности в результате слияния', '52', 0),
  ('Прекращение деятельности в результате разделения (раскола)', '53', 0),
  ('Прекращение деятельности в результате присоединения', '54', 0),
  ('Прекращение деятельности в результате преобразования', '55', 0),
  ('Сложные демографические события', '60', 0),
  ('Передача обособленного подразделения одним юридическим лицом другому юридическому лицу', '61', 0),
  ('Создание юридического лица на базе одного обособленного подразделения', '62', 0),
  ('Поглощение юридического лица с созданием на его основе обособленного подразделения', '63', 0),
  ('Изменение наименования', '70', 0),
  ('Изменение местонахождения', '71', 0),
  ('Изменение ФИО руководителя', '72', 0),
  ('Изменение органа управления (владельца)', '73', 0),
  ('Изменение основного вида деятельности', '74', 0),
  ('Изменение формы собственности', '75', 0),
  ('Изменение организационно- правовой формы', '76', 0),
  ('Изменение учредителя', '77', 0),
  ('Изменение уставного капитала', '78', 0),
  ('Изменение списочной численности', '79', 0),
  ('Ликвидация физического лица в результате хозяйственной несостоятельности', '90', 0)
