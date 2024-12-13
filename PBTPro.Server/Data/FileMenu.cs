namespace PBTPro.Data
{
    public class FileMenu
    {
        private static readonly Lazy<List<FileMenuObject>> fileElements = new Lazy<List<FileMenuObject>>(() => {
            List<FileMenuObject> content = new List<FileMenuObject>() {
                new FileMenuObject("Tetapan", "100", "0", "", true, 1, "", "root-item", new List<FileMenuObject>() {
                    new FileMenuObject("Pengguna Sistem", "101", "100", "user_system", true, 2, "/images/icons-small/xfn.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "101", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "101", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "101", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "101", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "101", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Peranan Sistem", "102", "100", "role", true, 3, "/images/icons-small/toolbox.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "102", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "102", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "102", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "102", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "102", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Pengguna & Peranan", "103", "100", "user_role", true, 4, "/images/icons-small/xfn-friend.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "103", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "103", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "103", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "103", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "103", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Akses Peranan", "104", "100", "user_access", true, 5, "/images/icons-small/blue-document-tree.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "104", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "104", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "104", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "104", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "104", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Jabatan", "105", "100", "senarai_jabatan", true, 6, "/images/icons-small/hard-hat.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "105", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "105", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "105", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "105", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "105", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Unit", "106", "100", "unit", true, 7, "/images/icons-small/hard-hat--arrow.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "106", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "106", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "106", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "106", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "106", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Negeri", "107", "100", "negeri", true, 8, "/images/icons-small/direction.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "107", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "107", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "107", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "107", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "107", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Daerah", "108", "100", "daerah", true, 9, "/images/icons-small/marker.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "108", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "108", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "108", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "108", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "108", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Mukim", "109", "100", "mukim", true, 10, "/images/icons-small/map-pin.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "109", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "109", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "109", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "109", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "109", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Setup Notis", "110", "100", "setup_notis", true, 11, "/images/icons-small/document-hf.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "110", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "110", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "110", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "110", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "110", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Setup Kompaun", "111", "100", "setup_kompaun", true, 11, "/images/icons-small/document-hf.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "111", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "111", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "111", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "111", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "111", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Setup Nota Pemeriksaan", "112", "100", "setup_nota_pemeriksaan", true, 11, "/images/icons-small/document-hf.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "112", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "112", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "112", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "112", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "112", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Setup Medan Borang", "113", "100", "setupBorang", true, 11, "/images/icons-small/document-hf.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "113", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "113", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "113", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "113", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "113", "", false, 5, "", "")
                    })
                }),
                new FileMenuObject("Papan Pemuka", "200", "0", "", true, 1, "", "root-item", new List<FileMenuObject>() {
                    new FileMenuObject("Ringkasan Eksekutif", "201", "200", "zon_eksekutif", true, 2, "/images/icons-small/table-heatmap.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "201", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "201", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "201", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "201", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "201", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Ringkasan Zon Majlis", "202", "200", "zon_majlis", true, 3, "/images/icons-small/table-insert.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "202", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "202", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "202", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "202", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "202", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Graf Statistik", "203", "200", "./", true, 4, "/images/icons-small/chart-up-color.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "203", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "203", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "203", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "203", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "203", "", false, 5, "", "")
                    })
                }),
                new FileMenuObject("Taburan Data", "300", "0", "", true, 1, "", "root-item", new List<FileMenuObject>() {
                    new FileMenuObject("Lesen", "301", "300", "taburan_lesen", true, 2, "/images/icons-small/tables-stacks.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "301", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "301", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "301", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "301", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "301", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Taksiran", "302", "300", "taburan_cukai", true, 3, "/images/icons-small/table-money.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "302", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "302", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "302", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "302", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "302", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Penguatkuasaan", "303", "300", "dashboard", true, 4, "/images/icons-small/table--pencil.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "303", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "303", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "303", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "303", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "303", "", false, 5, "", "")
                    })
                }),
                new FileMenuObject("Perincian Data", "400", "0", "./", true, 1, "/images/icons-small/blue-document-template.png", "singleNodesClass", new List<FileMenuObject>() {
                    new FileMenuObject("Papar", "1", "400", "", false, 1, "", ""),
                    new FileMenuObject("Tambah", "2", "400", "", false, 2, "", ""),
                    new FileMenuObject("Ubah", "3", "400", "", false, 3, "", ""),
                    new FileMenuObject("Hapus", "4", "400", "", false, 4, "", ""),
                    new FileMenuObject("Cetak", "5", "400", "", false, 5, "", "")
                }),
                new FileMenuObject("Nota Pemeriksaan", "500", "0", "./", true, 1, "/images/icons-small/blog--pencil.png", "singleNodesClass", new List<FileMenuObject>() {
                    new FileMenuObject("Papar", "1", "500", "", false, 1, "", ""),
                    new FileMenuObject("Tambah", "2", "500", "", false, 2, "", ""),
                    new FileMenuObject("Ubah", "3", "500", "", false, 3, "", ""),
                    new FileMenuObject("Hapus", "4", "500", "", false, 4, "", ""),
                    new FileMenuObject("Cetak", "5", "500", "", false, 5, "", "")
                }),
                new FileMenuObject("Pelaporan", "600", "0", "", true, 1, "", "root-item", new List<FileMenuObject>() {
                    new FileMenuObject("Laporan Kompaun", "601", "600", "kompaun_lesen", true, 2, "/images/icons-small/document-template.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "601", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "601", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "601", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "601", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "601", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Laporan Notis", "602", "600", "lapor_notis", true, 3, "/images/icons-small/document-template.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "602", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "602", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "602", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "602", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "602", "", false, 5, "", "")
                    }),
                    new FileMenuObject("Laporan Harian", "603", "600", "rptweatherforecast", true, 4, "/images/icons-small/document-template.png", "", new List<FileMenuObject>() {
                        new FileMenuObject("Papar", "1", "603", "", false, 1, "", ""),
                        new FileMenuObject("Tambah", "2", "603", "", false, 2, "", ""),
                        new FileMenuObject("Ubah", "3", "603", "", false, 3, "", ""),
                        new FileMenuObject("Hapus", "4", "603", "", false, 4, "", ""),
                        new FileMenuObject("Cetak", "5", "603", "", false, 5, "", "")
                    })
                }),
            };
            return content;
        });
        public static List<FileMenuObject> Content { get { return fileElements.Value; } }
    }
}
