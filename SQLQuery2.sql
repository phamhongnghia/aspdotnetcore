use BooksDatabase
create table Books(
	id int not null,
	tensp nvarchar(100),
	tacgia nvarchar(100),
	giagoc int,
	giamgia int,
	hinhanh nvarchar(500),
	primary key (id)
)
create table roleUser(
	idRole int not null,
	tenRole nvarchar(100),
	primary key (idRole)
)
drop table loaisanpham
create table loaisanpham(
	maloai int not null,
	tenloai nvarchar(100),
	primary key (maloai)
)
alter table Books add foreign key (maloai) references loaisanpham(maloai) 
create table khachhang(
	tendangnhap nvarchar(100) not null,
	matkhau nvarchar(100),
	hoten nvarchar(100),
	diachi nvarchar(200),
	idRole int,
	primary key (tendangnhap),
	foreign key (idRole) references roleUser
)
drop table detailBook
create table detailBook(
	id int not null,
	BookID int,
	nhacungcap nvarchar(100),
	nhaxuatban nvarchar(100),
	hinhthuc nvarchar(100),
	nguoidich nvarchar(100),
	mota text,
	noidung text,
	primary key (id),
	foreign key (BookID) references Books
)

create table giohang(
	id int not null,
	tendangnhap nvarchar(100),
	BookID int,
	tensp nvarchar(100),
	hinhanh nvarchar(100),
	giagoc int,
	giamgia int,
	soluong int,
	thanhtien int,
	primary key (id),
	foreign key (tendangnhap) references khachhang,
	foreign key (BookID) references Books
)

create table donhang(
	id int not null,
	tendangnhap nvarchar(100),
	BookID int,
	hoten nvarchar(100),
	diachi nvarchar(100),
	tensp nvarchar(100),
	ngaytao date,
	giagoc int,
	giamgia int,
	soluong int,
	thanhtien int,
	primary key (id),
	foreign key (tendangnhap) references khachhang,
	foreign key (BookID) references Books
)

CREATE procedure [dbo].[CheckUser]
	@Tendangnhap nvarchar(100),
	@Matkhau nvarchar(100)
as
begin
	select * from khachhang where tendangnhap = @Tendangnhap and matkhau = @Matkhau
end
GO

create proc [dbo].[FetchIdTypeBook]
	@ID int
as
begin
	select * from Books where maloai = @ID
end
go

select * from Books where maloai = 2

create proc [dbo].[FetchType]
	@ID int
as
begin
	select * from loaisanpham where maloai = @ID
end
go

select * from loaisanpham

CREATE procedure [dbo].[GetRole]
	@IdRole int
as
begin
	select * from roleUser where idRole = @IdRole
end
GO
select * from khachhang where tendangnhap = 'hongnghia' and matkhau = '123'

create procedure [dbo].[FetchDetail]
	@BookID int
as
begin
	select * from detailBook where BookID = @BookID
end
go

create procedure [dbo].[BookViewByID]
	@BookID int
as
begin
	select * from Books where BookID = @BookID
end
GO