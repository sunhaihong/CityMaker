using Gvitech.CityMaker.FdeCore;
using Gvitech.CityMaker.RenderControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityMakerHelper
{
    public class TreeHelp
    {
        /// <summary>
        /// 获取工程中所有的fc
        /// </summary>
        /// <param name="axRender"></param>
        /// <returns></returns>
        public static Hashtable GetHoleFcMap(Gvitech.CityMaker.Controls.AxRenderControl axRender)
        {
            Hashtable ht = new Hashtable();
            try
            {
                //根节点
                var root = axRender.ProjectTree.RootID;
                root = axRender.ProjectTree.GetNextItem(root, 12);
                ht = BuildTreeRecursive(axRender, ht, root);
            }
            catch (Exception ex)
            {

            }
            return ht;
        }
        private static Hashtable BuildTreeRecursive(Gvitech.CityMaker.Controls.AxRenderControl axRender, Hashtable ht, System.Guid current)
        {
            while (current.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                if (axRender.ProjectTree.IsGroup(current))
                {
                    string gname = axRender.ProjectTree.GetItemName(current);
                    var child = axRender.ProjectTree.GetNextItem(current, 11);
                    BuildTreeRecursive(axRender, ht, child);

                }
                else
                {
                    IRObject currentObj = axRender.ObjectManager.GetObjectById(current);
                    if (currentObj.Type == gviObjectType.gviObjectFeatureLayer)
                    {
                        IFeatureLayer layer = (IFeatureLayer)currentObj;
                        string ci = layer.FeatureClassInfo.DataSourceConnectionString;
                        IDataSourceFactory dsFactory = new DataSourceFactory();
                        IDataSource ds = dsFactory.OpenDataSourceByString(ci);
                        string[] setnames = (string[])ds.GetFeatureDatasetNames();
                        for (int i = 0; i < setnames.Length; i++)
                        {
                            IFeatureDataSet dataset = ds.OpenFeatureDataset(setnames[i]);
                            string[] fcnames = (string[])dataset.GetNamesByType(gviDataSetType.gviDataSetFeatureClassTable);
                            foreach (string name in fcnames)
                            {
                                IFeatureClass fc = dataset.OpenFeatureClass(name);
                                ht.Add(fc.Guid, fc);
                            }
                        }

                    }
                }
                current = axRender.ProjectTree.GetNextItem(current, 13);
            }
            return ht;

        }
    }
}
