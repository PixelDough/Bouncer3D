import bpy

class ExportWithLOD(bpy.types.Operator):
    """Export with LOD"""
    bl_idname = "export_with_lod.export"
    bl_label = "Export with LOD"
    
    @classmethod
    def poll(cls, context):
        return context.object is not None
    
    def execute(self, context):
        obj = context.active_object
        
        if obj.type == 'MESH':
            modifier = None
            for mod in obj.modifiers:
                if mod.type == 'SUBSURF':
                    modifier = mod
                    break
            
            if modifier:
                # Export with LODs
                for i in range(modifier.levels + 1):
                    new_obj = obj.copy()
                    new_mesh = obj.data.copy()
                    new_obj.data = new_mesh
                    bpy.context.collection.objects.link(new_obj)
                    new_obj.name = f"{obj.name}_LOD{modifier.levels - i}"
                    new_mesh.name = f"{obj.data.name}_LOD{modifier.levels - i}"
                    modifier_copy = new_obj.modifiers.new(name="Subsurf", type='SUBSURF')
                    modifier_copy.levels = i
                    modifier_copy.render_levels = i
                    modifier_copy.subdivision_type = modifier.subdivision_type
                # Export FBX
                filepath = bpy.path.ensure_ext(bpy.context.blend_data.filepath, ".fbx")
                bpy.ops.export_scene.fbx(filepath=filepath)
                return {'FINISHED'}
            else:
                self.report({'ERROR'}, "No Subdivision Surface modifier found")
                return {'CANCELLED'}
        else:
            self.report({'ERROR'}, "Selected object is not a mesh")
            return {'CANCELLED'}

def menu_func(self, context):
    self.layout.operator(ExportWithLOD.bl_idname, text="FBX with LOD")

def register():
    bpy.utils.register_class(ExportWithLOD)
    bpy.types.TOPBAR_MT_file_export.append(menu_func)

def unregister():
    bpy.utils.unregister_class(ExportWithLOD)
    bpy.types.TOPBAR_MT_file_export.remove(menu_func)

if __name__ == "__main__":
    register()
